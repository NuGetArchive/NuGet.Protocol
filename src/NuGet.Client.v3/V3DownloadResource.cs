using Newtonsoft.Json.Linq;
using NuGet.Configuration;
using NuGet.PackagingCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Data;
using System.Collections.Concurrent;

namespace NuGet.Client
{
    /// <summary>
    /// Provides the download metatdata for a given package from a V3 server endpoint.
    /// </summary>
    public class V3DownloadResource : DownloadResource
    {
        // cache all json retrieved in this resource, the resource *should* be thrown away after the operation is done
        private readonly ConcurrentDictionary<Uri, JObject> _cache; 
        
        private readonly HttpClient _client;
        private readonly Uri[] _downloadLinksTemplateUris;

        public V3DownloadResource(HttpClient client, Uri downloadLinksTemplateUri)
            : this(client, new[] { downloadLinksTemplateUri })
        {
        }

        public V3DownloadResource(HttpClient client, Uri[] downloadLinksTemplateUris)
            : base()
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            if (downloadLinksTemplateUris == null)
            {
                throw new ArgumentNullException("downloadLinksTemplateUris");
            }

            _downloadLinksTemplateUris = downloadLinksTemplateUris;
            _client = client;

            _cache = new ConcurrentDictionary<Uri, JObject>();
        }

        public override async Task<Uri> GetDownloadUrl(PackageIdentity identity, CancellationToken token)
        {
            Uri[] downloadLinksUris = Utility.ApplyPackageIdVersionToUriTemplate(_downloadLinksTemplateUris, identity.Id, identity.Version).ToArray();

            foreach (Uri downloadLinksUri in downloadLinksUris)
            {
                JObject downloadLinks = await GetJson(downloadLinksUri, token);

                if (downloadLinks != null)
                {
                    Uri packageContentUri = new Uri(downloadLinks["packageContent"].ToString());
                    HttpRequestMessage packageContentHeadRequest = new HttpRequestMessage(HttpMethod.Head, packageContentUri);

                    HttpResponseMessage headResponse = await _client.SendAsync(packageContentHeadRequest, token);

                    if (headResponse.IsSuccessStatusCode)
                    {
                        return packageContentUri;
                    }
                }
            }

            return null;
        }

        public override async Task<Stream> GetStream(PackageIdentity identity, CancellationToken token)
        {
            Stream stream = null;

            Uri uri = await GetDownloadUrl(identity, token);

            if (uri != null)
            {
                stream = await _client.GetStreamAsync(uri);
            }

            return stream;
        }

        /// <summary>
        /// Retrieve and cache json safely
        /// </summary>
        protected virtual async Task<JObject> GetJson(Uri uri, CancellationToken cancellationToken)
        {
            JObject json = null;
            if (!_cache.TryGetValue(uri, out json) || json == null)
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
                    json = null;
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
