using NuGet.Configuration;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace NuGet.Protocol
{
    /// <summary>
    /// A light weight XML parser for NuGet V2 Feeds
    /// </summary>
    public sealed class V2FeedParser
    {
        private const string W3Atom = "http://www.w3.org/2005/Atom";
        private const string MetadataNS = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";
        private const string DataServicesNS = "http://schemas.microsoft.com/ado/2007/08/dataservices";
        private const string FindPackagesByIdFormat = "/FindPackagesById()?Id='{0}'";

        // XNames used in the feed
        private static readonly XName _xnameEntry = XName.Get("entry", W3Atom);
        private static readonly XName _xnameTitle = XName.Get("title", W3Atom);
        private static readonly XName _xnameContent = XName.Get("content", W3Atom);
        private static readonly XName _xnameLink = XName.Get("link", W3Atom);
        private static readonly XName _xnameProperties = XName.Get("properties", MetadataNS);
        private static readonly XName _xnameId = XName.Get("Id", DataServicesNS);
        private static readonly XName _xnameVersion = XName.Get("Version", DataServicesNS);
        private static readonly XName _xnameSummary = XName.Get("summary", W3Atom);
        private static readonly XName _xnameDescription = XName.Get("Description", DataServicesNS);
        private static readonly XName _xnameIconUrl = XName.Get("IconUrl", DataServicesNS);
        private static readonly XName _xnameLicenseUrl = XName.Get("LicenseUrl", DataServicesNS);
        private static readonly XName _xnameProjectUrl = XName.Get("ProjectUrl", DataServicesNS);
        private static readonly XName _xnameTags = XName.Get("Tags", DataServicesNS);
        private static readonly XName _xnameReportAbuseUrl = XName.Get("ReportAbuseUrl", DataServicesNS);
        private static readonly XName _xnameDependencies = XName.Get("Dependencies", DataServicesNS);
        private static readonly XName _xnameRequireLicenseAcceptance = XName.Get("RequireLicenseAcceptance", DataServicesNS);
        private static readonly XName _xnameDownloadCount = XName.Get("DownloadCount", DataServicesNS);
        private static readonly XName _xnamePublished = XName.Get("Published", DataServicesNS);
        private static readonly XName _xnameName = XName.Get("name", W3Atom);
        private static readonly XName _xnameAuthor = XName.Get("author", W3Atom);

        private readonly HttpClient _httpClient;
        private readonly PackageSource _source;
        private readonly string _findPackagesByIdFormat;

        public V2FeedParser(HttpMessageHandler httpHandler, string sourceUrl)
            : this(httpHandler, new PackageSource(sourceUrl))
        {

        }

        /// <summary>
        /// Creates a V2 parser
        /// </summary>
        /// <param name="httpHandler">Message handler containing auth/proxy support</param>
        /// <param name="source">endpoint source</param>
        public V2FeedParser(HttpMessageHandler httpHandler, PackageSource source)
        {
            if (httpHandler == null)
            {
                throw new ArgumentNullException("httpClient");
            }

            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            _httpClient = new HttpClient(httpHandler);
            _source = source;
            _findPackagesByIdFormat = source.Source.TrimEnd('/') + FindPackagesByIdFormat;
        }

        /// <summary>
        /// Retrieves all packages with the given Id from a V2 feed.
        /// </summary>
        public async Task<IEnumerable<V2FeedPackageInfo>> FindPackagesByIdAsync(string id, CancellationToken token)
        {
            if (String.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id");
            }

            var uri = String.Format(CultureInfo.InvariantCulture, _findPackagesByIdFormat, id);
            var results = new List<V2FeedPackageInfo>();
            var page = 1;

            // first request
            Task<HttpResponseMessage> urlRequest = _httpClient.GetAsync(uri, token);

            // TODO: re-implement caching at a higher level for both v2 and v3
            while (!token.IsCancellationRequested && urlRequest != null)
            {
                // TODO: Pages for a package Id are cahced separately.
                // So we will get inaccurate data when a page shrinks.
                // However, (1) In most cases the pages grow rather than shrink;
                // (2) cache for pages is valid for only 30 min.
                // So we decide to leave current logic and observe.
                using (var data = await urlRequest)
                {
                    try
                    {
                        var doc = XDocument.Load(await data.Content.ReadAsStreamAsync());

                        // Example of what this looks like in the odata feed:
                        // <link rel="next" href="{nextLink}" />
                        var nextUri = (from e in doc.Root.Elements(_xnameLink)
                                       let attr = e.Attribute("rel")
                                       where attr != null && string.Equals(attr.Value, "next", StringComparison.OrdinalIgnoreCase)
                                       select e.Attribute("href") into nextLink
                                       where nextLink != null
                                       select nextLink.Value).FirstOrDefault();

                        // Request the next url in parallel to parsing the current page
                        urlRequest = null;
                        if (!string.IsNullOrEmpty(nextUri) && uri != nextUri)
                        {
                            // a bug on the server side causes the same next link to be returned 
                            // for every page. To avoid falling into an infinite loop we must
                            // keep track here.
                            uri = nextUri;

                            urlRequest = _httpClient.GetAsync(nextUri, token);
                        }

                        // find results on the page
                        var result = ParsePage(doc, id);
                        results.AddRange(result);

                        page++;
                    }
                    catch (XmlException)
                    {
                        //_reports.Information.WriteLine("The XML file {0} is corrupt",
                        //    data.CacheFileName.Yellow().Bold());
                        throw;
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Finds all entries on the page and parses them
        /// </summary>
        private IEnumerable<V2FeedPackageInfo> ParsePage(XDocument doc, string id)
        {
            var result = doc.Root
                .Elements(_xnameEntry)
                .Select(x => ParsePackage(id, x));

            return result;
        }

        /// <summary>
        /// Parse an entry into a V2FeedPackageInfo
        /// </summary>
        private V2FeedPackageInfo ParsePackage(string id, XElement element)
        {
            var properties = element.Element(_xnameProperties);
            var idElement = properties.Element(_xnameId);
            var titleElement = element.Element(_xnameTitle);

            // If 'Id' element exist, use its value as accurate package Id
            // Otherwise, use the value of 'title' if it exist
            // Use the given Id as final fallback if all elements above don't exist
            string identityId = idElement?.Value ?? titleElement?.Value ?? id;
            string versionString = properties.Element(_xnameVersion).Value;
            NuGetVersion version = NuGetVersion.Parse(versionString);
            string downloadUrl = element.Element(_xnameContent).Attribute("src").Value;

            string title = titleElement?.Value;
            string summary = GetValue(element, _xnameSummary);
            string description = GetValue(properties, _xnameDescription);
            string iconUrl = GetValue(properties, _xnameIconUrl);
            string licenseUrl = GetValue(properties, _xnameLicenseUrl);
            string projectUrl = GetValue(properties, _xnameProjectUrl);
            string reportAbuseUrl = GetValue(properties, _xnameReportAbuseUrl);
            string tags = GetValue(properties, _xnameTags);
            string dependencies = GetValue(properties, _xnameDependencies);

            string downloadCount = GetValue(properties, _xnameDownloadCount);
            bool requireLicenseAcceptance = GetValue(properties, _xnameRequireLicenseAcceptance) == "true";

            DateTimeOffset? published = null;

            DateTimeOffset pubVal = DateTimeOffset.MinValue;
            string pubString = GetValue(properties, _xnamePublished);
            if (DateTimeOffset.TryParse(pubString, out pubVal))
            {
                published = pubVal;
            }

            // TODO: is this ever populated in v2?
            IEnumerable<string> owners = null;

            IEnumerable<string> authors = null;

            var authorNode = element.Element(_xnameAuthor);
            if (authorNode != null)
            {
                authors = authorNode.Elements(_xnameName).Select(e => e.Value);
            }

            return new V2FeedPackageInfo(new PackageIdentity(identityId, version), 
                title, summary, description, authors, owners, iconUrl, licenseUrl, 
                projectUrl, reportAbuseUrl, tags, published, dependencies, 
                requireLicenseAcceptance, downloadUrl, downloadCount);
        }

        /// <summary>
        /// Retrieve an XML value safely
        /// </summary>
        private static string GetValue(XElement parent, XName childName)
        {
            string value = null;

            if (parent != null)
            {
                XElement child = parent.Element(childName);

                if (child != null)
                {
                    value = child.Value;
                }
            }

            return value;
        }
    }
}
