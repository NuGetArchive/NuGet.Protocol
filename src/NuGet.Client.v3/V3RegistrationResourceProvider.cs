using NuGet.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NuGet.Client
{
    [NuGetResourceProviderMetadata(typeof(V3RegistrationResource), "V3RegistrationResource", NuGetResourceProviderPositions.Last)]
    public class V3RegistrationResourceProvider : INuGetResourceProvider
    {
        public V3RegistrationResourceProvider()
        {
        }

        public async Task<INuGetResource> Create(SourceRepository source)
        {
            V3RegistrationResource regResource = null;
            var serviceIndex = await source.GetResource<V3ServiceIndexResource>();

            if (serviceIndex != null)
            {
                Uri baseUrl = serviceIndex["RegistrationsBaseUrl"].FirstOrDefault();

                DataClient client = new DataClient((await source.GetResource<HttpHandlerResource>()).MessageHandler);

                // construct a new resource
                regResource = new V3RegistrationResource(client, baseUrl);
            }

            return regResource;
        }
    }
}
