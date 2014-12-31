using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NuGet.Client
{
    /// <summary>
    /// Represents a Server endpoint. Exposes methods to get a specific resource like Search resoure, Metrics service and so on for the given server endpoint.
    /// This will be the replacement for existing SourceRepository class.
    /// </summary>  
    public class SourceRepository
    {
        private IEnumerable<Lazy<INuGetResourceProvider, INuGetResourceProviderMetadata>> _providers { get; set; }
        private readonly PackageSource _source;

        //*TODOs: Providers should be automatically imported when run inside vs context. Right now passing triggering it as part of testapp and passing it as param.
        public SourceRepository(PackageSource source, IEnumerable<Lazy<INuGetResourceProvider, INuGetResourceProviderMetadata>> providers)
        {
            _source = source;
            _providers = providers;
        }

        /// <summary>
        /// Package source
        /// </summary>
        public PackageSource Source
        {
            get
            {
                return _source;
            }
        }

        /// <summary>
        /// Returns a resource from the SourceRepository if it exists.
        /// </summary>
        /// <typeparam name="T">Expected resource type</typeparam>
        /// <returns>Null if the resource does not exist</returns>
        public async Task<T> GetResource<T>() 
        {
            INuGetResource resource = null;

            foreach (Lazy<INuGetResourceProvider, INuGetResourceProviderMetadata> provider in _providers)
            {
                //Each provider will expose the "ResourceType" that it can create. Filter the provider based on the current "resourceType" that is requested and invoke TryCreateResource on it.
                if (provider.Metadata.ResourceType == typeof(T))
                {
                    if (provider.Value.TryCreate(this, out resource))
                    {
                        // found
                        break;
                    }
                }
            }

            if (resource != null)
            {
                return (T)resource;
            }

            return default(T);
        }
    }
}
