using NuGet.Client.V3;
using NuGet.Client.VisualStudio;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Client.V3.VisualStudio
{
    [Export(typeof(INuGetResourceProvider))]
    [ResourceProviderMetadata(typeof(UISearch))]
    public class VsV3SearchResourceProvider : INuGetResourceProvider
    {
        public VsV3SearchResourceProvider()
        {

        }

        public bool TryCreate(SourceRepository source, out INuGetResource resource)
        {
            throw new NotImplementedException();
        }
    }
}
