using Newtonsoft.Json.Linq;
using NuGet.Data;
using NuGet.PackagingCore;
using NuGet.Versioning;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace NuGet.Client
{
    /// <summary>
    /// Registration blob reader
    /// </summary>
    public class V3ResolverPackageIndexResource : INuGetResource
    {
        // cache all json retrieved in this resource, the resource *should* be thrown away after the operation is done
        private readonly ConcurrentDictionary<Uri, JObject> _cache;

        private readonly HttpClient _client;
        private readonly Uri[] _indexTemplateUris;

        private static readonly VersionRange AllVersions = new VersionRange(null, true, null, true, true);

        public V3ResolverPackageIndexResource(HttpClient client, Uri[] indexTemplateUris)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            if (indexTemplateUris == null || !indexTemplateUris.Any())
            {
                throw new ArgumentNullException("indexTemplateUri");
            }

            _client = client;
            _indexTemplateUris = indexTemplateUris;

            _cache = new ConcurrentDictionary<Uri, JObject>();
        }

        /// <summary>
        /// Returns the registration blob for the id and version
        /// </summary>
        /// <remarks>The inlined entries are potentially going away soon</remarks>
        public virtual async Task<JObject> GetPackageMetadata(PackageIdentity identity, CancellationToken token)
        {
            return (await GetPackageMetadata(identity.Id, new VersionRange(identity.Version, true, identity.Version, true), true, true, token)).SingleOrDefault();
        }

        /// <summary>
        /// Returns inlined catalog entry items for each registration blob
        /// </summary>
        /// <remarks>The inlined entries are potentially going away soon</remarks>
        public virtual async Task<IEnumerable<JObject>> GetPackageMetadata(string packageId, bool includePrerelease, bool includeUnlisted, CancellationToken token)
        {
            return await GetPackageMetadata(packageId, AllVersions, includePrerelease, includeUnlisted, token);
        }

        /// <summary>
        /// Returns inlined catalog entry items for each registration blob
        /// </summary>
        /// <remarks>The inlined entries are potentially going away soon</remarks>
        public virtual async Task<IEnumerable<JObject>> GetPackageMetadata(string packageId, VersionRange range, bool includePrerelease, bool includeUnlisted, CancellationToken token)
        {
            List<JObject> results = new List<JObject>();

            var entries = await GetPackageEntries(packageId, includeUnlisted, token);

            foreach (var entry in entries)
            {
                JToken catalogEntry = entry["catalogEntry"];

                if (catalogEntry != null)
                {
                    NuGetVersion version = null;

                    if (catalogEntry["version"] != null && NuGetVersion.TryParse(catalogEntry["version"].ToString(), out version))
                    {
                        if (range.Satisfies(version) && (includePrerelease || !version.IsPrerelease))
                        {
                            if (catalogEntry["published"] != null)
                            {
                                DateTime published = catalogEntry["published"].ToObject<DateTime>();

                                if ((published != null && published.Year > 1901) || includeUnlisted)
                                {
                                    // add in the download url
                                    if (entry["packageContent"] != null)
                                    {
                                        catalogEntry["packageContent"] = entry["packageContent"];
                                    }

                                    results.Add(entry["catalogEntry"] as JObject);
                                }
                            }
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Returns catalog:CatalogPage items
        /// </summary>
        public virtual async Task<IEnumerable<JObject>> GetPages(string packageId, CancellationToken token)
        {
            List<JObject> results = new List<JObject>();

            JObject indexJson = await GetIndex(packageId, token);

            var items = indexJson["items"] as JArray;

            if (items != null)
            {
                foreach (var item in items)
                {
                    if (item["@type"] != null && StringComparer.Ordinal.Equals(item["@type"].ToString(), "catalog:CatalogPage"))
                    {
                        if (item["items"] != null)
                        {
                            // normal inline page
                            results.Add(item as JObject);
                        }
                        else
                        {
                            // fetch the page
                            string url = item["@id"].ToString();

                            JObject catalogPage = await GetJson(new Uri(url), token);

                            results.Add(catalogPage);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Returns all index entries of type Package within the given range and filters
        /// </summary>
        public virtual async Task<IEnumerable<JObject>> GetPackageEntries(string packageId, bool includeUnlisted, CancellationToken token)
        {
            List<JObject> results = new List<JObject>();

            var pages = await GetPages(packageId, token);

            foreach (JObject catalogPage in pages)
            {
                JArray array = catalogPage["items"] as JArray;

                if (array != null)
                {
                    foreach (JToken item in array)
                    {
                        if (item["@type"] != null && StringComparer.Ordinal.Equals(item["@type"].ToString(), "Package"))
                        {
                            // TODO: listed check
                            results.Add(item as JObject);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Returns the index.json registration page for a package.
        /// </summary>
        public virtual async Task<JObject> GetIndex(string packageId, CancellationToken cancellationToken)
        {
            foreach (var indexTemplateUri in _indexTemplateUris)
            {
                var indexUri = Utility.ApplyPackageIdToUriTemplate(indexTemplateUri, packageId);

                if (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        return await GetJson(indexUri, cancellationToken);
                    }
                    catch (Exception)
                    {
                        Debug.Fail("Registration Index GET failed");
                    }
                }
            }

            throw new NuGetProtocolException(Strings.Protocol_MissingResolverPackageIndexMetadataTemplateUri);
        }

        /// <summary>
        /// Retrieve and cache json safely
        /// </summary>
        protected virtual async Task<JObject> GetJson(Uri uri, CancellationToken cancellationToken)
        {
            JObject json = null;
            if (!_cache.TryGetValue(uri, out json))
            {
                var response = await _client.GetAsync(uri, cancellationToken);
                var status = (int)response.StatusCode;

                // ignore missing blobs
                if (response.IsSuccessStatusCode)
                {
                    // throw on bad files
                    json = JObject.Parse(await response.Content.ReadAsStringAsync());
                }
                else if (status >= 400 && status < 500)
                {
                    // This was a request error, which generally indicates that the
                    // package cannot be found (usually a 404 would happen here).
                    // Cache an empty object so we don't continually retry
                    json = new JObject();
                }
                else
                {
                    // Server error would fall into here
                    // Throw in those cases because the server is unavailable
                    throw new NuGetProtocolException(response.ReasonPhrase);
                }

                _cache.TryAdd(uri, json);
            }

            return json;
        }
    }
}
