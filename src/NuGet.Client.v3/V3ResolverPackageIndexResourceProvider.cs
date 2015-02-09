using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Data;

namespace NuGet.Client
{
    [NuGetResourceProviderMetadata(typeof(V3ResolverPackageIndexResource), "V3ResolverPackageIndexResource", NuGetResourceProviderPositions.Last)]
    public class V3ResolverPackageIndexResourceProvider : INuGetResourceProvider
    {
        public V3ResolverPackageIndexResourceProvider()
        {
        }

        public async Task<Tuple<bool, INuGetResource>> TryCreate(SourceRepository source, CancellationToken token)
        {
            V3ResolverPackageIndexResource regResource = null;
            var serviceIndex = await source.GetResourceAsync<V3ServiceIndexResource>(token);

            if (serviceIndex != null)
            {
                Uri[] indexUris = serviceIndex[ServiceTypes.ResolverPackageIndexMetadataTemplateUri].ToArray();

                var messageHandlerResource = await source.GetResourceAsync<HttpHandlerResource>(token);

                DataClient client = new DataClient(messageHandlerResource.MessageHandler);

                // construct a new resource
                regResource = new V3ResolverPackageIndexResource(client, indexUris);
            }

            return new Tuple<bool, INuGetResource>(regResource != null, regResource);
        }
    }
}