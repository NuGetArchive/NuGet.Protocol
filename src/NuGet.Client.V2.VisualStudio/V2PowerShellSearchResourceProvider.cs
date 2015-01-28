using NuGet.Client.VisualStudio;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace NuGet.Client.V2.VisualStudio
{
    
    [NuGetResourceProviderMetadata(typeof(PSSearchResource), "V2PSSearchResourceProvider", NuGetResourceProviderPositions.Last)]
    public class V2PSSearchResourceProvider : V2ResourceProvider
    {
        private readonly ConcurrentDictionary<Configuration.PackageSource, PSSearchResource> _cache = new ConcurrentDictionary<Configuration.PackageSource, PSSearchResource>();

        public override async Task<INuGetResource> Create(SourceRepository source)
        {
            INuGetResource resource = null;
            PSSearchResource v2PSSearchResource;
            if (!_cache.TryGetValue(source.PackageSource, out v2PSSearchResource))
            {
                UISearchResource uiSearchResource = await source.GetResource<UISearchResource>();
                if (uiSearchResource != null)
                {
                    v2PSSearchResource = new V2PowerShellSearchResource(uiSearchResource);
                    _cache.TryAdd(source.PackageSource, v2PSSearchResource);
                    resource = v2PSSearchResource;
                }
                else
                {
                    resource = null;
                }
            }
            else
            {
                resource = v2PSSearchResource;
            }

            return resource;
        }
    }
}
