using NuGet.Client.VisualStudio;
using NuGet.Data;
using NuGet.PackagingCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Client.V3.VisualStudio
{
    public class V3UIMetadataResource : UIMetadataResource
    {
        public V3UIMetadataResource(DataClient client)
            : base()
        {

        }

        public override async Task<IEnumerable<UIPackageMetadata>> GetMetadata(IEnumerable<PackageIdentity> packages)
        {
            throw new NotImplementedException();
        }
    }
}
