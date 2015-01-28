using NuGet.Client.VisualStudio;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace NuGet.Client.V2.VisualStudio
{    
    [NuGetResourceProviderMetadata(typeof(UISearchResource), "V2UISearchResourceProvider", NuGetResourceProviderPositions.Last)]
    public class V2UISearchResourceProvider : V2ResourceProvider
    {
        private readonly ConcurrentDictionary<Configuration.PackageSource, UISearchResource> _cache = new ConcurrentDictionary<Configuration.PackageSource,UISearchResource>();

        public override async Task<INuGetResource> Create(SourceRepository source)
        {
            INuGetResource resource = null;
            UISearchResource v2UISearchResource;
            if (!_cache.TryGetValue(source.PackageSource, out v2UISearchResource))
            {
                resource = await base.Create(source);
                if (resource != null)
                {

                    v2UISearchResource = new V2UISearchResource((V2Resource)resource);
                    _cache.TryAdd(source.PackageSource, v2UISearchResource);
                    resource = v2UISearchResource;
                }
                else
                {
                    resource = null;
                }
            }
            else
            {
                resource = v2UISearchResource;
            }

            return resource;
        }
    }
}