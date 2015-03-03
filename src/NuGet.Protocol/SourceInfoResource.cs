using System;
using System.Threading.Tasks;

namespace NuGet.Protocol
{
    /// <summary>
    /// A central location for determining resource type. Other resources may exist that are aware of more 
    /// types. This is just a simple to use resource for the most common ones.
    /// </summary>
    public class SourceInfoResource : INuGetResource
    {
        private readonly string _source;

        public SourceInfoResource(string source)
        {
            _source = source;
        }

        public string Source
        {
            get
            {
                return _source;
            }
        }

        public bool IsHttp
        {
            get
            {
                return _source.StartsWith("http", StringComparison.OrdinalIgnoreCase);
            }
        }


        public async Task<bool> IsServerV3()
        {
            // TODO: improve this
            return IsHttp && _source.EndsWith(".json", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<bool> IsServerV2()
        {
            // TODO: improve this
            return IsHttp && await IsServerV3() == false;
        }

        public Task<bool> IsPackagesFolderV2()
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsPackagesFolderV3()
        {
            throw new NotImplementedException();
        }
    }
}