using NuGet.PackagingCore;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Client
{
    public abstract class MetadataResource : INuGetResource
    {
        public abstract Task<IEnumerable<NuGetVersion>> GetLatestVersions(IEnumerable<string> packageIds, bool includePrerelease = false, bool includeUnlisted = false);

        public abstract Task<IEnumerable<KeyValuePair<string, bool>>> IsSatellitePackage(IEnumerable<string> packageId);
    }
}
