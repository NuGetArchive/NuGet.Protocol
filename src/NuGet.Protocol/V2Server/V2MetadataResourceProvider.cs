using System;
using System.Threading;
using System.Threading.Tasks;

namespace NuGet.Protocol
{
    public class V2MetadataResourceProvider : ResourceProvider
    {
        public V2MetadataResourceProvider()
            : base(typeof(MetadataResource), "V2MetadataResourceProvider", NuGetResourceProviderPositions.Last)
        {

        }

        public override async Task<Tuple<bool, INuGetResource>> TryCreate(SourceRepository source, CancellationToken token)
        {
            MetadataResource resource = null;
            var sourceInfo = await source.GetResourceAsync<SourceInfoResource>();

            if (await sourceInfo.IsServerV2())
            {
                var httpHandler = await source.GetResourceAsync<HttpHandlerResource>();

                resource = new V2MetadataResource(new V2FeedParser(httpHandler.MessageHandler, source.PackageSource));
            }

            return new Tuple<bool, INuGetResource>(resource != null, resource);
        }
    }
}
