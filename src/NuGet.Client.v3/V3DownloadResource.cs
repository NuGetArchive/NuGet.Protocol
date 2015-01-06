using Newtonsoft.Json.Linq;
using NuGet.Configuration;
using NuGet.PackagingCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Client
{
    /// <summary>
    /// Provides the download metatdata for a given package from a V3 server endpoint.
    /// </summary>
    public class V3DownloadResource : DownloadResource
    {
        public V3DownloadResource(HttpClient client, PackageSource source)
            : base()
        {

        }

        public override Task<Uri> GetDownloadUrl(PackageIdentity identity)
        {
            throw new NotImplementedException();
        }
    }
}
