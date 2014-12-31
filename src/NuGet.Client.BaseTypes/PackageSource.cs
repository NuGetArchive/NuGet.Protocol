using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Internal.Utils;

namespace NuGet.Client
{
    public class PackageSource
    {
        private readonly string _name;
        private readonly string _url;

        public PackageSource(string name, string url)
        {
            _name = name;
            _url = url;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string Url
        {
            get
            {
                return _url;
            }
        }

        public override bool Equals(object obj)
        {
            PackageSource other = obj as PackageSource;
            if (other == null)
            {
                return false;
            }

            return String.Equals(Name, other.Name, StringComparison.CurrentCultureIgnoreCase) &&
                String.Equals(Url, other.Url, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return HashCodeCombiner.Start()
                .Add(Name)
                .Add(Url)
                .CombinedHash;
        }

        public override string ToString()
        {
            return Name + ": " + Url;
        }
    }
}
