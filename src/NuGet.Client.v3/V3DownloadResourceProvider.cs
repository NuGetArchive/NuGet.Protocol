using NuGet.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Client
{
    [Export(typeof(INuGetResourceProvider))]
    [ResourceProviderMetadata(typeof(DownloadResource))]
    public class V3DownloadResourceProvider : INuGetResourceProvider
    {
        private readonly ConcurrentDictionary<PackageSource, DownloadResource> _cache;

        public V3DownloadResourceProvider()
        {
            _cache = new ConcurrentDictionary<PackageSource, DownloadResource>();
        }

        public bool TryCreate(SourceRepository source, out INuGetResource resource)
        {
            DownloadResource downloadResource = null;

            if (source.GetResource<V3ServiceIndexResource>() != null)
            {
                if (!_cache.TryGetValue(source.Source, out downloadResource))
                {
                    downloadResource = new V3DownloadResource(new DataClient(), source.Source);

                    _cache.TryAdd(source.Source, downloadResource);
                }
            }

            resource = downloadResource;
            return downloadResource != null;
        }
    }
}
