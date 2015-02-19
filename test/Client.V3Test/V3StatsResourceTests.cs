using System;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Client;
using NuGet.PackagingCore;
using NuGet.Versioning;
using Xunit;

namespace Client.V3Test
{
    public class V3StatsResourceTests : TestBase
    {
        [Fact]
        public async Task DownloadResource_Found()
        {
            //var resource = await SourceRepository.GetResourceAsync<V3StatsResource>();
            var resource = new V3StatsResource(DataClient, new Uri("https://nugetgallery.blob.core.windows.net/v3-stats0/")); // todo: remove and replace with line above when the index.json has this service type listed
            
            var res = await resource.GetTotalStats(CancellationToken.None);

            Assert.NotNull(res);
        }
    }
}