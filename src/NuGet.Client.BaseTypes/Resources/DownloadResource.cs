using NuGet.PackagingCore;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Client
{
    public abstract class DownloadResource : INuGetResource
    {
        public abstract Task<Uri> GetDownloadUrl(PackageIdentity identity);
    }
}
