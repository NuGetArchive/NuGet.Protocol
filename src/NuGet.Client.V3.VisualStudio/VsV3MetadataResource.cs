using Newtonsoft.Json.Linq;
using NuGet.Client;
using NuGet.Client.V3;
using NuGet.Client.VisualStudio.Models;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Client.V3.VisualStudio
{
    public class VsV3MetadataResource : V3Resource, IVsMetadata
    {
        public VsV3MetadataResource(V3Resource v3Resource)
            : base(v3Resource) { }

        public async Task<VisualStudioUIPackageMetadata> GetPackageMetadataForVisualStudioUI(string packageId, NuGetVersion version)
        {
           JObject metatdata =  await V3Client.GetPackageMetadata(packageId, version);
           return GetVisualstudioPackageMetadata(metatdata);           
        }

        private VisualStudioUIPackageMetadata GetVisualstudioPackageMetadata(JObject metadata)
        {
           
            NuGetVersion Version = NuGetVersion.Parse(metadata.Value<string>(Properties.Version));
            string publishedStr = metadata.Value<string>(Properties.Published);
            DateTimeOffset? Published= null;
            if (!String.IsNullOrEmpty(publishedStr))
            {
                Published = DateTime.Parse(publishedStr);
            }

            string Summary = metadata.Value<string>(Properties.Summary);
            string Description = metadata.Value<string>(Properties.Description);
            string Authors = metadata.Value<string>(Properties.Authors);
            string Owners = metadata.Value<string>(Properties.Owners);
            Uri IconUrl = GetUri(metadata, Properties.IconUrl);
            Uri LicenseUrl = GetUri(metadata, Properties.LicenseUrl);
            Uri ProjectUrl = GetUri(metadata, Properties.ProjectUrl);
            string Tags = String.Join(" ", (metadata.Value<JArray>(Properties.Tags) ?? Enumerable.Empty<JToken>()).Select(t => t.ToString()));
            int DownloadCount = metadata.Value<int>(Properties.DownloadCount);
           IEnumerable<VisualStudioUIPackageDependencySet> DependencySets = (metadata.Value<JArray>(Properties.DependencyGroups) ?? Enumerable.Empty<JToken>()).Select(obj => LoadDependencySet((JObject)obj));

            bool HasDependencies = DependencySets.Any(
                set => set.Dependencies != null && set.Dependencies.Count > 0);

            return new VisualStudioUIPackageMetadata(Version, Summary, Description, Authors, Owners, IconUrl, LicenseUrl, ProjectUrl, Tags, DownloadCount, Published, DependencySets, HasDependencies);
        }
        private Uri GetUri(JObject json, string property)
        {
            if (json[property] == null)
            {
                return null;
            }
            string str = json[property].ToString();
            if (String.IsNullOrEmpty(str))
            {
                return null;
            }
            return new Uri(str);
        }

        private static VisualStudioUIPackageDependencySet LoadDependencySet(JObject set)
        {
            var fxName = set.Value<string>(Properties.TargetFramework);
            //TODOs: Temp frameworkName. Need to use the version utility properly. It's up to the client to process it.
            return new VisualStudioUIPackageDependencySet(
                String.IsNullOrEmpty(fxName) ? null : new FrameworkName(fxName), 
                (set.Value<JArray>(Properties.Dependencies) ?? Enumerable.Empty<JToken>()).Select(obj => LoadDependency((JObject)obj)));
        }

        private static VisualStudioUIPackageDependency LoadDependency(JObject dep)
        {
            var ver = dep.Value<string>(Properties.Range);
            return new VisualStudioUIPackageDependency(
                dep.Value<string>(Properties.PackageId),
                String.IsNullOrEmpty(ver) ? null : VersionRange.Parse(ver));
        }
    }
}
