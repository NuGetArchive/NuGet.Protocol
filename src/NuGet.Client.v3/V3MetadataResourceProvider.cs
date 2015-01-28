using NuGet.Data;
using System.Threading.Tasks;

namespace NuGet.Client
{
    [NuGetResourceProviderMetadata(typeof(MetadataResource), "V3MetadataResourceProvider", "V2MetadataResourceProvider")]
    public class V3MetadataResourceProvider : INuGetResourceProvider
    {
        public V3MetadataResourceProvider()
        {
        }

        public async Task<INuGetResource> Create(SourceRepository source)
        {
            V3MetadataResource curResource = null;
            V3RegistrationResource regResource = await source.GetResource<V3RegistrationResource>();

            if (regResource != null)
            {
                DataClient client = new DataClient((await source.GetResource<HttpHandlerResource>()).MessageHandler);

                curResource = new V3MetadataResource(client, regResource);
            }

            return curResource;
        }
    }
}
