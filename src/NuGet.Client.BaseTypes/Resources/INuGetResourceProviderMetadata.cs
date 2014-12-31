using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Client
{
    public interface INuGetResourceProviderMetadata
    {
         Type ResourceType { get; }
    }
}
