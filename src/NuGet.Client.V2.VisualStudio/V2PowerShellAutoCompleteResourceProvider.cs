using NuGet.Client.VisualStudio;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace NuGet.Client.V2.VisualStudio
{
    
    [NuGetResourceProviderMetadata(typeof(PSAutoCompleteResource), "V2PowerShellAutoCompleteResourceProvider", NuGetResourceProviderPositions.Last)]
    public class V2PowerShellAutoCompleteResourceProvider : V2ResourceProvider
    {
        private readonly ConcurrentDictionary<Configuration.PackageSource, PSAutoCompleteResource> _cache = new ConcurrentDictionary<Configuration.PackageSource,PSAutoCompleteResource>();

        public override async Task<INuGetResource> Create(SourceRepository source)
        {
            INuGetResource resource = null;
            PSAutoCompleteResource v2PowerShellAutoCompleteResource;
            if (!_cache.TryGetValue(source.PackageSource, out v2PowerShellAutoCompleteResource))
            {
                resource = await base.Create(source);
                if (resource != null)
                {
                    v2PowerShellAutoCompleteResource = new V2PowerShellAutoCompleteResource((V2Resource)resource);
                    _cache.TryAdd(source.PackageSource, v2PowerShellAutoCompleteResource);
                    resource = v2PowerShellAutoCompleteResource;
                }
                else
                {
                    resource = null;
                }
            }
            else
            {
                resource = v2PowerShellAutoCompleteResource;
            }

            return resource;
        }
    }
}
