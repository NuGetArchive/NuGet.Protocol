using NuGet.Protocol;
using System;
using System.Net.Http;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Xml.Linq;
using System.Linq;
using NuGet.Versioning;

namespace ProtocolTests
{
    public class V2FeedParserTests
    {
        [Fact]
        public async Task V2FeedParser_Basic()
        {
            HttpClient client = new HttpClient(new TestHandler());

            V2FeedParser parser = new V2FeedParser(client, "http://testsource/v2/");

            var packages = await parser.FindPackagesByIdAsync("WindowsAzure.Storage", CancellationToken.None);

            Assert.Equal(34, packages.Count());
        }

        [Fact]
        public async Task V2FeedParser_FollowNextLinks()
        {
            HttpClient client = new HttpClient(new TestHandler());

            V2FeedParser parser = new V2FeedParser(client, "http://testsource/v2/");

            var packages = await parser.FindPackagesByIdAsync("ravendb.client", CancellationToken.None);

            Assert.Equal(300, packages.Count());
        }

        [Fact]
        public async Task V2FeedParser_PackageInfo()
        {
            HttpClient client = new HttpClient(new TestHandler());

            V2FeedParser parser = new V2FeedParser(client, "http://testsource/v2/");

            var packages = await parser.FindPackagesByIdAsync("WindowsAzure.Storage", CancellationToken.None);

            var latest = packages.OrderByDescending(e => e.Version, VersionComparer.VersionRelease).FirstOrDefault();

            Assert.Equal("WindowsAzure.Storage", latest.Id);
            Assert.Equal("4.3.2-preview", latest.Version.ToNormalizedString());
            Assert.Null(latest.Title);
            Assert.Equal("", latest.Authors);
            Assert.Equal("", latest.Description);
            Assert.Equal(0, latest.DownloadCount);
            Assert.Equal("", latest.DownloadUrl);
            Assert.Equal("", latest.IconUrl);
            Assert.Equal("", latest.LicenseUrl);
            Assert.Equal("", latest.Owners);
            Assert.Equal("", latest.ProjectUrl);
            Assert.Equal("", latest.Published.Value.ToString("O"));
            Assert.Equal("", latest.ReportAbuseUrl);
            Assert.True(latest.RequireLicenseAcceptance);
            Assert.Equal("", latest.Summary);
            Assert.Equal("", latest.Tags);
            Assert.Equal(1, latest.DependencySets.Count());
        }


        private class TestHandler : HttpMessageHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                HttpResponseMessage msg = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

                if (request.RequestUri.AbsoluteUri == "http://testsource/v2/FindPackagesById()?Id='WindowsAzure.Storage'")
                {
                    msg.Content = new TestContent(FindPackagesByIdData.WindowsAzureStorage);
                }
                else if (request.RequestUri.AbsoluteUri == "http://testsource/v2/FindPackagesById()?Id='ravendb.client'")
                {
                    msg.Content = new TestContent(FindPackagesByIdData.MultiPage1);
                }
                else if (request.RequestUri.AbsoluteUri == "http://api.nuget.org/api/v2/FindPackagesById?id='ravendb.client'&$skiptoken='RavenDB.Client','1.2.2067-Unstable'")
                {
                    msg.Content = new TestContent(FindPackagesByIdData.MultiPage2);
                }
                else if (request.RequestUri.AbsoluteUri == "http://api.nuget.org/api/v2/FindPackagesById?id='ravendb.client'&$skiptoken='RavenDB.Client','2.5.2617-Unstable'")
                {
                    msg.Content = new TestContent(FindPackagesByIdData.MultiPage3);
                }
                else
                {
                    msg = new HttpResponseMessage(HttpStatusCode.NotFound);
                }

                return msg;
            }
        }

        private class TestContent : HttpContent
        {
            private MemoryStream _stream;

            public TestContent(string xml)
            {
                var doc = XDocument.Parse(xml);
                _stream = new MemoryStream();
                doc.Save(_stream);
                _stream.Seek(0, SeekOrigin.Begin);
            }

            protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                _stream.CopyTo(stream);
            }

            protected override bool TryComputeLength(out long length)
            {
                length = (long)_stream.Length;
                return true;
            }
        }
    }
}