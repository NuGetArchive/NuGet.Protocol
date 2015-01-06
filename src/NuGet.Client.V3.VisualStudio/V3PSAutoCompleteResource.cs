using NuGet.Client.VisualStudio;
using NuGet.Data;
using NuGet.PackagingCore;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Client.V3.VisualStudio
{
    public class V3PSAutoCompleteResource : PSAutoCompleteResource
    {
        public V3PSAutoCompleteResource(DataClient client)
            : base()
        {

        }

        public override async Task<IEnumerable<string>> GetPackageIdsStartingWith(string packageIdPrefix, bool includePrerelease = false)
        {
            throw new NotImplementedException();
        }

        public override async Task<IEnumerable<NuGetVersion>> GetPackageVersionsStartingWith(string versionPrefix, bool includePrerelease = false)
        {
            throw new NotImplementedException();
        }
    }
}
