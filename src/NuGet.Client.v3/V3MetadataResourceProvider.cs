using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Client
{
    [Export(typeof(INuGetResourceProvider))]
    [ResourceProviderMetadata(typeof(MetadataResource))]
    public class V3MetadataResourceProvider : INuGetResourceProvider
    {
        public V3MetadataResourceProvider()
        {

        }

        public bool TryCreate(SourceRepository source, out INuGetResource resource)
        {
            resource = null;
            return false;
        }
    }
}
