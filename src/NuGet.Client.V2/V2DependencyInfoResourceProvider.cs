using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace NuGet.Client.V2
{
    
    [NuGetResourceProviderMetadata(typeof(DepedencyInfoResource), "V2DependencyInfoResourceProvider", NuGetResourceProviderPositions.Last)]
    public class V2DependencyInfoResourceProvider : V2ResourceProvider
    {
        private readonly ConcurrentDictionary<Configuration.PackageSource, DepedencyInfoResource> _cache = new ConcurrentDictionary<Configuration.PackageSource, DepedencyInfoResource>();

        public override async Task<INuGetResource> Create(SourceRepository source)
        {
            INuGetResource resource = null;
            DepedencyInfoResource v2DependencyInfoResource;
            if (!_cache.TryGetValue(source.PackageSource, out v2DependencyInfoResource))
            {
                resource = await base.Create(source);
                if (resource != null)
                {
                    v2DependencyInfoResource = new V2DependencyInfoResource((V2Resource)resource);
                    _cache.TryAdd(source.PackageSource, v2DependencyInfoResource);
                    resource = v2DependencyInfoResource;
                }
                else
                {
                    resource = null;
                }
            }
            else
            {
                resource = v2DependencyInfoResource;
            }

            return resource;
        }
    }
}
