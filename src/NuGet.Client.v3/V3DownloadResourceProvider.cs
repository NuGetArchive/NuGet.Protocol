using NuGet.Configuration;
using NuGet.Data;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace NuGet.Client
{
    [NuGetResourceProviderMetadata(typeof(DownloadResource), "V3DownloadResourceProvider", "V2DownloadResourceProvider")]
    public class V3DownloadResourceProvider : INuGetResourceProvider
    {
        private readonly ConcurrentDictionary<PackageSource, DownloadResource> _cache;

        public V3DownloadResourceProvider()
        {
            _cache = new ConcurrentDictionary<PackageSource, DownloadResource>();
        }

        public async Task<INuGetResource> Create(SourceRepository source)
        {
            DownloadResource downloadResource = null;

            var serviceIndex = await source.GetResource<V3ServiceIndexResource>();

            if (serviceIndex != null)
            {
                if (!_cache.TryGetValue(source.PackageSource, out downloadResource))
                {
                    var registrationResource = await source.GetResource<V3RegistrationResource>();

                    DataClient client = new DataClient((await source.GetResource<HttpHandlerResource>()).MessageHandler);

                    downloadResource = new V3DownloadResource(client, registrationResource);

                    _cache.TryAdd(source.PackageSource, downloadResource);
                }
            }

            return downloadResource;
        }
    }
}
