// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using NuGet.Versioning;
using System.Net.Http;
using NuGet.Configuration;
using System.Globalization;
using System.Threading;
using NuGet.Packaging.Core;
using NuGet.Packaging;

namespace NuGet.Protocol
{
    public class V2FeedParser
    {
        private const string W3Atom = "http://www.w3.org/2005/Atom";
        private const string MetadataNS = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";
        private const string DataServicesNS = "http://schemas.microsoft.com/ado/2007/08/dataservices";
        private const string FindPackagesByIdFormat = "/FindPackagesById()?Id='{0}'";

        private static readonly XName _xnameEntry = XName.Get("entry", W3Atom);
        private static readonly XName _xnameTitle = XName.Get("title", W3Atom);
        private static readonly XName _xnameContent = XName.Get("content", W3Atom);
        private static readonly XName _xnameLink = XName.Get("link", W3Atom);
        private static readonly XName _xnameProperties = XName.Get("properties", MetadataNS);
        private static readonly XName _xnameId = XName.Get("Id", DataServicesNS);
        private static readonly XName _xnameVersion = XName.Get("Version", DataServicesNS);

        private readonly HttpClient _httpClient;
        private readonly PackageSource _source;
        private readonly string _findPackagesByIdFormat;

        public V2FeedParser(HttpClient httpClient, string sourceUrl)
            : this(httpClient, new PackageSource(sourceUrl))
        {

        }

        public V2FeedParser(HttpClient httpClient, PackageSource source)
        {
            if (httpClient == null)
            {
                throw new ArgumentNullException("httpClient");
            }

            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            _httpClient = httpClient;
            _source = source;
            _findPackagesByIdFormat = source.Source.TrimEnd('/') + FindPackagesByIdFormat;
        }

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

        private IEnumerable<V2FeedPackageInfo> ParsePage(XDocument doc, string id)
        {
            var result = doc.Root
                .Elements(_xnameEntry)
                .Select(x => ParsePackage(id, x));

            return result;
        }

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

            string title = null;
            string summary = null;
            string authors = null;
            string description = null;
            string owners = null;
            string iconUrl = null;
            string licenseUrl = null;
            string projectUrl = null;
            string reportAbuseUrl = null;
            string tags = null;
            int downloadCount = 0;
            bool requireLicenseAcceptance = false;
            DateTimeOffset? published = null;
            IEnumerable<PackageDependencyGroup> dependencySets = Enumerable.Empty<PackageDependencyGroup>();

            return new V2FeedPackageInfo(new PackageIdentity(identityId, version), 
                title, summary, description, authors, owners, iconUrl, licenseUrl, 
                projectUrl, reportAbuseUrl, tags, published, dependencySets, 
                requireLicenseAcceptance, downloadUrl, downloadCount);
        }
    }
}
