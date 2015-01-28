using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace NuGet.Client.V2
{

    [NuGetResourceProviderMetadata(typeof(SimpleSearchResource), "V2SimpleSearchResourceProvider", NuGetResourceProviderPositions.Last)]
    public class V2SimpleSearchResourceProvider : V2ResourceProvider
    {
        private readonly ConcurrentDictionary<Configuration.PackageSource, SimpleSearchResource> _cache = new ConcurrentDictionary<Configuration.PackageSource, SimpleSearchResource>();

        public override async Task<INuGetResource> Create(SourceRepository source)
        {
            INuGetResource resource = null;
            SimpleSearchResource v2SimpleSearchResource;
            if (!_cache.TryGetValue(source.PackageSource, out v2SimpleSearchResource))
            {
                resource = await base.Create(source);
                if (resource != null)
                {

                    v2SimpleSearchResource = new V2SimpleSearchResource((V2Resource)resource);
                    _cache.TryAdd(source.PackageSource, v2SimpleSearchResource);
                    resource = v2SimpleSearchResource;
                }
                else
                {
                    resource = null;
                }
            }
            else
            {
                resource = v2SimpleSearchResource;
            }

            return resource;
        }
    }
}
