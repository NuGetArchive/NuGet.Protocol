using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Client.VisualStudio
{
    public abstract class PowerShellAutoComplete : INuGetResource
    {
       public abstract Task<IEnumerable<string>> GetPackageIdsStartingWith(string packageIdPrefix, bool includePrerelease=false);

       public abstract Task<IEnumerable<NuGetVersion>> GetPackageVersionsStartingWith(string versionPrefix, bool includePrerelease = false);
    }
}

