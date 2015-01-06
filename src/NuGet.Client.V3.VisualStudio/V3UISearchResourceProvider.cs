using NuGet.Client.V3;
using NuGet.Client.VisualStudio;
using NuGet.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Client.V3.VisualStudio
{
    [Export(typeof(INuGetResourceProvider))]
    [ResourceProviderMetadata(typeof(UISearchResource))]
    public class V3UISearchResourceProvider : INuGetResourceProvider
    {
        private readonly DataClient _client;

        public V3UISearchResourceProvider()
            : this(new DataClient())
        {

        }

        public V3UISearchResourceProvider(DataClient client)
        {
            _client = client;
        }

        public bool TryCreate(SourceRepository source, out INuGetResource resource)
        {
            V3UISearchResource curResource = null;

            if (source.GetResource<V3ServiceIndexResource>() != null)
            {
                // construct a new resource
                curResource = new V3UISearchResource(_client);
            }

            resource = curResource;
            return resource != null;
        }
    }
}
