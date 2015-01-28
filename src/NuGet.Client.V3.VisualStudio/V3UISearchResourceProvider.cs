using NuGet.Client.VisualStudio;
using NuGet.Data;
using System.Threading.Tasks;

namespace NuGet.Client.V3.VisualStudio
{
    [NuGetResourceProviderMetadata(typeof(UISearchResource), "V3UISearchResourceProvider", "V2UISearchResourceProvider")]
    public class V3UISearchResourceProvider : INuGetResourceProvider
    {
        private readonly DataClient _client;

        public V3UISearchResourceProvider()
            : this(new DataClient())
        {

        }

        public V3UISearchResourceProvider(DataClient client)
        {
            _client = client;
        }

        public async Task<INuGetResource> Create(SourceRepository source)
        {
            V3UISearchResource curResource = null;
            V3ServiceIndexResource serviceIndex = await source.GetResource<V3ServiceIndexResource>();

            if (serviceIndex != null)
            {
                var rawSearch = await source.GetResource<V3RawSearchResource>();
                curResource = new V3UISearchResource(rawSearch);
            }

            return curResource;
        }
    }
}
