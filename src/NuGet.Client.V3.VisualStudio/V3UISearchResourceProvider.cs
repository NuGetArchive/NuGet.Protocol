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
            V3ServiceIndexResource serviceIndex = source.GetResource<V3ServiceIndexResource>();

            if (serviceIndex != null)
            {
                // TODO: take this work around out and use _serviceIndex.Index["SearchQueryService"] - this is just because the package hasn't been updated yet!
                var endpoints = serviceIndex.Index["resources"].Where(j => ((string)j["@type"]) == "SearchQueryService").Select(o => o["@id"].ToObject<Uri>()).ToArray();

                if (endpoints.Length > 0)
                {
                    // construct a new resource
                    curResource = new V3UISearchResource(_client, endpoints);
                }
            }

            resource = curResource;
            return resource != null;
        }
    }
}
