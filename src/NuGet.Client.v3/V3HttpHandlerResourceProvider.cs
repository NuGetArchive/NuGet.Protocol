using NuGet.Data;
using System.Threading.Tasks;

namespace NuGet.Client
{
    [NuGetResourceProviderMetadata(typeof(HttpHandlerResource), "V3HttpHandlerResourceProvider", NuGetResourceProviderPositions.Last)]
    public class V3HttpHandlerResourceProvider : INuGetResourceProvider
    {
        public V3HttpHandlerResourceProvider()
        {
        }

        public async Task<INuGetResource> Create(SourceRepository source)
        {
            // Everyone gets a dataclient
            return await Task.FromResult(new V3HttpHandlerResource(DataClient.DefaultHandler));
        }
    }
}
