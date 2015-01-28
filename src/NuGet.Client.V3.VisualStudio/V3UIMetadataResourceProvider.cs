using NuGet.Client.VisualStudio;
using NuGet.Data;
using System.Threading.Tasks;

namespace NuGet.Client.V3.VisualStudio
{
    [NuGetResourceProviderMetadata(typeof(UIMetadataResource), "V3UIMetadataResourceProvider", "V2UIMetadataResourceProvider")]
    public class V3UIMetadataResourceProvider : INuGetResourceProvider
    {
        private readonly DataClient _client;

        public V3UIMetadataResourceProvider()
            : this(new DataClient())
        {

        }

        public V3UIMetadataResourceProvider(DataClient client)
        {
            _client = client;
        }

        public async Task<INuGetResource> Create(SourceRepository source)
        {
            V3UIMetadataResource curResource = null;

            if (await source.GetResource<V3ServiceIndexResource>() != null)
            {
                var regResource = await source.GetResource<V3RegistrationResource>();

                // construct a new resource
                curResource = new V3UIMetadataResource(_client, regResource);
            }

            return curResource;
        }
    }
}
