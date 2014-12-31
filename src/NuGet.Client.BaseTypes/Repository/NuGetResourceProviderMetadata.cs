using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Client
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ResourceProviderMetadata : ExportAttribute, INuGetResourceProviderMetadata
    {
        private readonly Type _resourceType;

        public ResourceProviderMetadata(Type resourceType)
            : base(typeof(INuGetResourceProvider))
        {
            _resourceType = resourceType;
        }

        public Type ResourceType
        {
            get
            {
                return _resourceType;
            }
        }
    }
}
