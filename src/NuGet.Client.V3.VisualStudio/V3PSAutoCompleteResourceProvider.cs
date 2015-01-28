using NuGet.Client.VisualStudio;
using NuGet.Data;
using System.Threading.Tasks;

namespace NuGet.Client.V3.VisualStudio
{
    [NuGetResourceProviderMetadata(typeof(PSAutoCompleteResource), "V3PSAutoCompleteResourceProvider", "V2PSAutoCompleteResourceProvider")]
    public class V3PSAutoCompleteResourceProvider : INuGetResourceProvider
    {
        private readonly DataClient _client;

        public V3PSAutoCompleteResourceProvider()
            : this(new DataClient())
        {

        }

        public V3PSAutoCompleteResourceProvider(DataClient client)
        {
            _client = client;
        }

        public async Task<INuGetResource> Create(SourceRepository source)
        {
            V3PSAutoCompleteResource curResource = null;

            var serviceIndex = await source.GetResource<V3ServiceIndexResource>();

            if (serviceIndex != null)
            {
                var regResource = await source.GetResource<V3RegistrationResource>();

                // construct a new resource
                curResource = new V3PSAutoCompleteResource(_client, serviceIndex, regResource);
            }

            return curResource;
        }
    }
}
