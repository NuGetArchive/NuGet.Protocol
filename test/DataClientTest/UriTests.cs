using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Data;
using NuGet.Versioning;
using Xunit;
using Xunit.Extensions;

namespace DataTest
{
    public class UriTests
    {
        /// <summary>
        /// Test out our package uri template processing with all of the tokens we support.
        /// </summary>
        /// <remarks>
        /// Be sure to check for case preservation along with the lower-case version of id/version.
        /// </remarks>
        [Theory]
        [InlineData("http://foo.com/", "packageId", "1.0.0-Beta", "http://foo.com/")]
        [InlineData("http://foo.com/{id}", "packageId", "1.0.0-Beta", "http://foo.com/packageId")]
        [InlineData("http://foo.com/{id-lower}", "packageId", "1.0.0-Beta", "http://foo.com/packageid")]
        [InlineData("http://foo.com/{version}", "packageId", "1.0.0-Beta", "http://foo.com/1.0.0-Beta")]
        [InlineData("http://foo.com/{version-lower}", "packageId", "1.0.0-Beta", "http://foo.com/1.0.0-beta")]
        [InlineData("http://foo.com/{id}/{version}", "packageId", "1.0.0-Beta", "http://foo.com/packageId/1.0.0-Beta")]
        [InlineData("http://foo.com/{id-lower}/{version-lower}", "packageId", "1.0.0-Beta", "http://foo.com/packageid/1.0.0-beta")]
        [InlineData("http://foo.com/query?id={id}&version={version}", "packageId", "1.0.0-Beta", "http://foo.com/query?id=packageId&version=1.0.0-Beta")]
        public void ApplyPackageToUriTemplate(string template, string id, string version, string expected)
        {
            var nugetVersion = new NuGetVersion(version);
            var actual = Utility.ApplyPackageToUriTemplate(new Uri(template), id, nugetVersion);
            Assert.Equal(expected, actual.ToString());
        }
    }
}
