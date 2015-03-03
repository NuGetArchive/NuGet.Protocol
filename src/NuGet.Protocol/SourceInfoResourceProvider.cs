using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NuGet.Protocol
{
    public class SourceInfoResourceProvider : ResourceProvider
    {
        // this cache exists globally, the type of a source should only be determined once
        private static ConcurrentDictionary<string, SourceInfoResource> _globalCache;

        public SourceInfoResourceProvider()
            : base(typeof(SourceInfoResource))
        {
            if (_globalCache == null)
            {
                _globalCache = new ConcurrentDictionary<string, SourceInfoResource>(StringComparer.OrdinalIgnoreCase);
            }
        }

        public override async Task<Tuple<bool, INuGetResource>> TryCreate(SourceRepository source, CancellationToken token)
        {
            SourceInfoResource resource = _globalCache.GetOrAdd(source.PackageSource.Source, new SourceInfoResource(source.PackageSource.Source));

            return new Tuple<bool, INuGetResource>(true, resource);
        }
    }
}