using NuGet.Data;
using System.Threading.Tasks;

namespace NuGet.Client
{
    /// <summary>
    /// Retrieves all dependency info for the package resolver.
    /// </summary>

    [NuGetResourceProviderMetadata(typeof(DepedencyInfoResource), "V3DependencyInfoResourceProvider", "V2DependencyInfoResourceProvider")]
    public class V3DependencyInfoResourceProvider : INuGetResourceProvider
    {
        public V3DependencyInfoResourceProvider()
        {
        }

        public async Task<INuGetResource> Create(SourceRepository source)
        {
            DepedencyInfoResource dependencyInfoResource = null;

            if (await source.GetResource<V3ServiceIndexResource>() != null)
            {
                DataClient client = new DataClient((await source.GetResource<HttpHandlerResource>()).MessageHandler);

                var regResource = await source.GetResource<V3RegistrationResource>();

                // construct a new resource
                dependencyInfoResource = new V3DependencyInfoResource(client, regResource);
            }

            return dependencyInfoResource;
        }
    }
}
