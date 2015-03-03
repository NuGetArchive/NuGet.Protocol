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
            V2FeedParser parser = new V2FeedParser(new TestHandler(), "http://testsource/v2/");

            var packages = await parser.FindPackagesByIdAsync("WindowsAzure.Storage", CancellationToken.None);

            Assert.Equal(34, packages.Count());
        }

        [Fact]
        public async Task V2FeedParser_FollowNextLinks()
        {
            V2FeedParser parser = new V2FeedParser(new TestHandler(), "http://testsource/v2/");

            var packages = await parser.FindPackagesByIdAsync("ravendb.client", CancellationToken.None);

            Assert.Equal(300, packages.Count());
        }

        [Fact]
        public async Task V2FeedParser_PackageInfo()
        {
            V2FeedParser parser = new V2FeedParser(new TestHandler(), "http://testsource/v2/");

            var packages = await parser.FindPackagesByIdAsync("WindowsAzure.Storage", CancellationToken.None);

            var latest = packages.OrderByDescending(e => e.Version, VersionComparer.VersionRelease).FirstOrDefault();

            Assert.Equal("WindowsAzure.Storage", latest.Id);
            Assert.Equal("4.3.2-preview", latest.Version.ToNormalizedString());
            Assert.Equal("WindowsAzure.Storage", latest.Title);
            Assert.Equal("Microsoft", String.Join(",", latest.Authors));
            Assert.Equal("", String.Join(",", latest.Owners));
            Assert.True(latest.Description.StartsWith("This client library enables"));
            Assert.Equal(2102565, latest.DownloadCountAsInt);
            Assert.Equal("http://api.nuget.org/api/v2/package/WindowsAzure.Storage/4.3.2-preview", latest.DownloadUrl);
            Assert.Equal("http://go.microsoft.com/fwlink/?LinkID=288890", latest.IconUrl);
            Assert.Equal("http://go.microsoft.com/fwlink/?LinkId=331471", latest.LicenseUrl);
            Assert.Equal("http://go.microsoft.com/fwlink/?LinkId=235168", latest.ProjectUrl);
            Assert.Equal(DateTimeOffset.Parse("2014-11-12T22:19:16.297"), latest.Published.Value);
            Assert.Equal("http://www.nuget.org/package/ReportAbuse/WindowsAzure.Storage/4.3.2-preview", latest.ReportAbuseUrl);
            Assert.True(latest.RequireLicenseAcceptance);
            Assert.Equal("A client library for working with Microsoft Azure storage services including blobs, files, tables, and queues.", latest.Summary);
            Assert.Equal("Microsoft Azure Storage Table Blob File Queue Scalable windowsazureofficial", latest.Tags);
            Assert.Equal("Microsoft.Data.OData:5.6.3:aspnetcore50|Microsoft.Data.Services.Client:5.6.3:aspnetcore50|System.Spatial:5.6.3:aspnetcore50|System.Collections:4.0.10-beta-22231:aspnetcore50|System.Collections.Concurrent:4.0.0-beta-22231:aspnetcore50|System.Collections.Specialized:4.0.0-beta-22231:aspnetcore50|System.Diagnostics.Debug:4.0.10-beta-22231:aspnetcore50|System.Diagnostics.Tools:4.0.0-beta-22231:aspnetcore50|System.Diagnostics.TraceSource:4.0.0-beta-22231:aspnetcore50|System.Diagnostics.Tracing:4.0.10-beta-22231:aspnetcore50|System.Dynamic.Runtime:4.0.0-beta-22231:aspnetcore50|System.Globalization:4.0.10-beta-22231:aspnetcore50|System.IO:4.0.10-beta-22231:aspnetcore50|System.IO.FileSystem:4.0.0-beta-22231:aspnetcore50|System.IO.FileSystem.Primitives:4.0.0-beta-22231:aspnetcore50|System.Linq:4.0.0-beta-22231:aspnetcore50|System.Linq.Expressions:4.0.0-beta-22231:aspnetcore50|System.Linq.Queryable:4.0.0-beta-22231:aspnetcore50|System.Net.Http:4.0.0-beta-22231:aspnetcore50|System.Net.Primitives:4.0.10-beta-22231:aspnetcore50|System.Reflection:4.0.10-beta-22231:aspnetcore50|System.Reflection.Extensions:4.0.0-beta-22231:aspnetcore50|System.Reflection.TypeExtensions:4.0.0-beta-22231:aspnetcore50|System.Runtime:4.0.20-beta-22231:aspnetcore50|System.Runtime.Extensions:4.0.10-beta-22231:aspnetcore50|System.Runtime.InteropServices:4.0.20-beta-22231:aspnetcore50|System.Runtime.Serialization.Primitives:4.0.0-beta-22231:aspnetcore50|System.Runtime.Serialization.Xml:4.0.10-beta-22231:aspnetcore50|System.Security.Cryptography.Encoding:4.0.0-beta-22231:aspnetcore50|System.Security.Cryptography.Encryption:4.0.0-beta-22231:aspnetcore50|System.Security.Cryptography.Hashing:4.0.0-beta-22231:aspnetcore50|System.Security.Cryptography.Hashing.Algorithms:4.0.0-beta-22231:aspnetcore50|System.Text.Encoding:4.0.10-beta-22231:aspnetcore50|System.Text.Encoding.Extensions:4.0.10-beta-22231:aspnetcore50|System.Text.RegularExpressions:4.0.10-beta-22231:aspnetcore50|System.Threading:4.0.0-beta-22231:aspnetcore50|System.Threading.Tasks:4.0.10-beta-22231:aspnetcore50|System.Threading.Thread:4.0.0-beta-22231:aspnetcore50|System.Threading.ThreadPool:4.0.10-beta-22231:aspnetcore50|System.Threading.Timer:4.0.0-beta-22231:aspnetcore50|System.Xml.ReaderWriter:4.0.10-beta-22231:aspnetcore50|System.Xml.XDocument:4.0.0-beta-22231:aspnetcore50|System.Xml.XmlSerializer:4.0.0-beta-22231:aspnetcore50|Microsoft.Data.OData:5.6.3:aspnet50|Microsoft.Data.Services.Client:5.6.3:aspnet50|System.Spatial:5.6.3:aspnet50|Microsoft.Data.OData:5.6.2:net40-Client|Newtonsoft.Json:5.0.8:net40-Client|Microsoft.Data.Services.Client:5.6.2:net40-Client|Microsoft.WindowsAzure.ConfigurationManager:1.8.0.0:net40-Client|Microsoft.Data.OData:5.6.2:win80|Microsoft.Data.OData:5.6.2:wpa|Microsoft.Data.OData:5.6.2:wp80|Newtonsoft.Json:5.0.8:wp80", latest.Dependencies);
            Assert.Equal(6, latest.DependencySets.Count());
            Assert.Equal("aspnetcore5", latest.DependencySets.First().TargetFramework.GetShortFolderName());
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