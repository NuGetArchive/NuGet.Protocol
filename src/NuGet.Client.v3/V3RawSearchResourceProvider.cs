using System.Linq;
using System.Threading.Tasks;

namespace NuGet.Client
{
    [NuGetResourceProviderMetadata(typeof(V3RawSearchResource), "V3RawSearchResource", NuGetResourceProviderPositions.Last)]
    public class V3RawSearchResourceProvider : INuGetResourceProvider
    {
        public V3RawSearchResourceProvider()
        {

        }

        public async Task<INuGetResource> Create(SourceRepository source)
        {
            V3RawSearchResource curResource = null;
            V3ServiceIndexResource serviceIndex = await source.GetResource<V3ServiceIndexResource>();

            if (serviceIndex != null)
            {
                var endpoints = serviceIndex["SearchQueryService"].ToArray();

                if (endpoints.Length > 0)
                {
                    HttpHandlerResource handler = await source.GetResource<HttpHandlerResource>();

                    // construct a new resource
                    curResource = new V3RawSearchResource(handler.MessageHandler, endpoints);
                }
            }

            return curResource;
        }
    }
}

