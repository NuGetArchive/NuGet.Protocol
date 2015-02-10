using NuGet.Client;
using NuGet.PackagingCore;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Client.V3Test
{
    public class RegistrationResourceTests : TestBase
    {
        private readonly Uri ResolverPackageIndexTemplateUri = new Uri("https://api.nuget.org/v3/registrations-1/{id}/index.json");

        [Fact]
        public async Task RegistrationResource_NotFound()
        {
            V3ResolverPackageIndexResource resource = new V3ResolverPackageIndexResource(DataClient, ResolverPackageIndexTemplateUri);

            var package = await resource.GetResolverMetadata("notfound23lk4j23lk432j4l", includePrerelease: true, includeUnlisted: true, token: CancellationToken.None);

            Assert.Null(package);
        }

        [Fact]
        public async Task RegistrationResource_Tree()
        {
            V3ResolverPackageIndexResource resource = new V3ResolverPackageIndexResource(DataClient, ResolverPackageIndexTemplateUri);

            var packages = await resource.GetResolverMetadata("ravendb.client", true, false, CancellationToken.None);

            var results = packages.ToArray();

            Assert.True(results.Length > 500);
        }

        [Fact]
        public async Task RegistrationResource_TreeFilterOnPre()
        {
            V3ResolverPackageIndexResource resource = new V3ResolverPackageIndexResource(DataClient, ResolverPackageIndexTemplateUri);

            var packages = await resource.GetResolverMetadata("ravendb.client", false, false, CancellationToken.None);

            var results = packages.ToArray();

            Assert.True(results.Length < 500);
        }

        [Fact]
        public async Task RegistrationResource_NonTree()
        {
            V3ResolverPackageIndexResource resource = new V3ResolverPackageIndexResource(DataClient, ResolverPackageIndexTemplateUri);

            var packagesPre = await resource.GetResolverMetadata("newtonsoft.json", true, false, CancellationToken.None);
            var packages = await resource.GetResolverMetadata("newtonsoft.json", false, false, CancellationToken.None);

            var results = packages.ToArray();
            var resultsPre = packagesPre.ToArray();

            Assert.True(results.Length > 10);
            Assert.True(results.Length < resultsPre.Length);
        }
    }
}
