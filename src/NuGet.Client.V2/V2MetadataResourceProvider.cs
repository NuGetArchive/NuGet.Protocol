using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace NuGet.Client.V2
{

    [NuGetResourceProviderMetadata(typeof(MetadataResource), "V2MetadataResourceProvider", NuGetResourceProviderPositions.Last)]
    public class V2MetadataResourceProvider : V2ResourceProvider
    {
        private readonly ConcurrentDictionary<Configuration.PackageSource, MetadataResource> _cache = new ConcurrentDictionary<Configuration.PackageSource, MetadataResource>();

        public override async Task<INuGetResource> Create(SourceRepository source)
        {
            INuGetResource resource = null;
            MetadataResource v2MetadataResource;
            if (!_cache.TryGetValue(source.PackageSource, out v2MetadataResource))
            {
                resource = await base.Create(source);
                if (resource != null)
                {

                    v2MetadataResource = new V2MetadataResource((V2Resource)resource);
                    _cache.TryAdd(source.PackageSource, v2MetadataResource);
                    resource = v2MetadataResource;
                }
                else
                {
                    resource = null;
                }
            }
            else
            {
                resource = v2MetadataResource;
            }

            return resource;
        }
    }
}
