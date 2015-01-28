using NuGet.Client.VisualStudio;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace NuGet.Client.V2.VisualStudio
{
    
    [NuGetResourceProviderMetadata(typeof(UIMetadataResource))]
    public class V2UIMetadataResourceProvider : V2ResourceProvider
    {
        private readonly ConcurrentDictionary<Configuration.PackageSource, UIMetadataResource> _cache = new ConcurrentDictionary<Configuration.PackageSource, UIMetadataResource>();

        public override async Task<INuGetResource> Create(SourceRepository source)
        {
            INuGetResource resource = null;
            UIMetadataResource v2UIMetadataResource;
            if (!_cache.TryGetValue(source.PackageSource, out v2UIMetadataResource))
            {
                resource = await base.Create(source);
                if (resource != null)
                {
                    v2UIMetadataResource = new V2UIMetadataResource((V2Resource)resource);
                    _cache.TryAdd(source.PackageSource, v2UIMetadataResource);
                    resource = v2UIMetadataResource;
                }
                else
                {
                    resource = null;
                }               
            }
            else
            {
                resource = v2UIMetadataResource;                
            }

            return resource;
        }
    }
}
