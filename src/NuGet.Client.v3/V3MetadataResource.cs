using Newtonsoft.Json.Linq;
using NuGet.Configuration;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Client
{
    public class V3MetadataResource : MetadataResource
    {
        public V3MetadataResource(HttpClient client, PackageSource source)
            : base()
        {

        }

        public override Task<IEnumerable<NuGetVersion>> GetLatestVersions(IEnumerable<string> packageIds, bool includePrerelease = false, bool includeUnlisted = false)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<KeyValuePair<string, bool>>> IsSatellitePackage(IEnumerable<string> packageId)
        {
            throw new NotImplementedException();
        }
    }
}
