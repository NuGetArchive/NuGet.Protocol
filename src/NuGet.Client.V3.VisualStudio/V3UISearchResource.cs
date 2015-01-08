using Newtonsoft.Json.Linq;
using NuGet.Client;
using NuGet.Client.V3;
using NuGet.Client.VisualStudio;
using NuGet.Data;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace NuGet.Client.V3.VisualStudio
{
    public class V3UISearchResource : UISearchResource
    {
        private readonly DataClient _client;
        private readonly Uri[] _searchEndpoints;

        public V3UISearchResource(DataClient client, IEnumerable<Uri> searchEndpoints)
            : base()
        {
            _client = client;
            _searchEndpoints = searchEndpoints.ToArray();
        }

        public override async Task<IEnumerable<UISearchMetadata>> Search(string searchTerm, SearchFilter filters, int skip, int take, CancellationToken cancellationToken)
        {
            for (int i = 0; i < _searchEndpoints.Length; i++)
            {
                var endpoint = _searchEndpoints[i];

                if (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        JObject searchJson = await _client.GetJObjectAsync(endpoint, cancellationToken);

                        if (searchJson != null)
                        {

                        }
                    }
                    catch (Exception)
                    {
                        Debug.Fail("Search failed");

                        if (i == _searchEndpoints.Length - 1)
                        {
                            // throw on the last one
                            throw;
                        }
                    }
                }
            }

            // TODO: localize message
            throw new NuGetProtocolException("Unable to search");
        }
    }
}
