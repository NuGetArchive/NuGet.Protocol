using NuGet.Client;
using NuGet.PackagingCore;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Client.V3Test
{
    public class DownloadResourceTests : TestBase
    {
        private readonly Uri DownloadLinksTemplateUri = new Uri("https://api.nuget.org/v3/registrations-1/{id-lower}/{version-lower}.json", UriKind.Absolute);

        [Fact]
        public async Task DownloadResource_NotFound()
        {
            V3DownloadResource resource = new V3DownloadResource(DataClient, DownloadLinksTemplateUri);

            var uri = await resource.GetDownloadUrl(new PackageIdentity("notfound23lk4j23lk432j4l", new NuGetVersion(1, 0, 99)), CancellationToken.None);

            Assert.Null(uri);

            var stream = await resource.GetStream(new PackageIdentity("notfound23lk4j23lk432j4l", new NuGetVersion(1, 0, 99)), CancellationToken.None);

            Assert.Null(stream);
        }

        [Fact]
        public async Task DownloadResource_Found()
        {
            V3DownloadResource resource = new V3DownloadResource(DataClient, DownloadLinksTemplateUri);

            var uri = await resource.GetDownloadUrl(new PackageIdentity("newtonsoft.json", new NuGetVersion(6, 0, 4)), CancellationToken.None);

            Assert.NotNull(uri);

            var stream = await resource.GetStream(new PackageIdentity("newtonsoft.json", new NuGetVersion(6, 0, 4)), CancellationToken.None);

            Assert.NotNull(stream);
        }

    }
}
