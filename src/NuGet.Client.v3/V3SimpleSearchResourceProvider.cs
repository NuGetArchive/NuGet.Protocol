using System.Threading.Tasks;

namespace NuGet.Client
{
    /// <summary>
    /// V3 Simple search resource aimed at command line searches
    /// </summary>

    [NuGetResourceProviderMetadata(typeof(SimpleSearchResource), "V3SimpleSearchResourceProvider", "V2SimpleSearchResourceProvider")]
    public class V3SimpleSearchResourceProvider : INuGetResourceProvider
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public V3SimpleSearchResourceProvider()
        {
        }

        public async Task<INuGetResource> Create(SourceRepository source)
        {
            V3SimpleSearchResource curResource = null;

            var rawSearch = await source.GetResource<V3RawSearchResource>();

            if (rawSearch != null && rawSearch is V3RawSearchResource)
            {
                curResource = new V3SimpleSearchResource(rawSearch);
            }

            return curResource;
        }
    }
}
