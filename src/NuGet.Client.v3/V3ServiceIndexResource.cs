using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Client
{
    /// <summary>
    /// Stores/caches a service index json file.
    /// </summary>
    public class V3ServiceIndexResource : INuGetResource
    {
        private readonly JObject _index;

        public V3ServiceIndexResource(JObject index)
        {
            _index = index;
        }

        public JObject Index
        {
            get
            {
                return _index;
            }
        }
    }
}
