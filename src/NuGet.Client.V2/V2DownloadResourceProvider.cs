using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace NuGet.Client.V2
{
    /// <summary>
    /// Resource provider for V2 download.
    /// </summary>

    [NuGetResourceProviderMetadata(typeof(DownloadResource), "V2DownloadResourceProvider", NuGetResourceProviderPositions.Last)]
    public class V2DownloadResourceProvider : V2ResourceProvider
    {
        private readonly ConcurrentDictionary<Configuration.PackageSource, DownloadResource> _cache = new ConcurrentDictionary<Configuration.PackageSource, DownloadResource>();

        public override async Task<INuGetResource> Create(SourceRepository source)
        {
            INuGetResource resource = null;
            DownloadResource v2DownloadResource;
            if (!_cache.TryGetValue(source.PackageSource, out v2DownloadResource))
            {
                resource = await base.Create(source);
                if (resource != null)
                {

                    v2DownloadResource = new V2DownloadResource((V2Resource)resource);
                    _cache.TryAdd(source.PackageSource, v2DownloadResource);
                    resource = v2DownloadResource;
                }
                else
                {
                    resource = null;
                }
            }
            else
            {
                resource = v2DownloadResource;
            }

            return resource;
        }
    }
}
