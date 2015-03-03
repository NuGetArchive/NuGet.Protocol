using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NuGet.Protocol
{
    /// <summary>
    /// Represents a V2 package entry from the OData feed. This object primarily just holds the strings parsed from XML, all parsing 
    /// and converting should be done after based on the scenario.
    /// </summary>
    public class V2FeedPackageInfo : PackageIdentity
    {
        private readonly string _title;
        private readonly string _summary;
        private readonly string[] _authors;
        private readonly string _description;
        private readonly string[] _owners;
        private readonly string _iconUrl;
        private readonly string _licenseUrl;
        private readonly string _projectUrl;
        private readonly string _reportAbuseUrl;
        private readonly string _tags;
        private readonly string _downloadCount;
        private readonly bool _requireLicenseAcceptance;
        private readonly DateTimeOffset? _published;
        private readonly string _dependencies;
        private readonly string _downloadUrl;

        public V2FeedPackageInfo(PackageIdentity identity, string title, string summary, string description, IEnumerable<string> authors, IEnumerable<string> owners,
            string iconUrl, string licenseUrl, string projectUrl, string reportAbuseUrl,
            string tags, DateTimeOffset? published, string dependencies, bool requireLicenseAccept, string downloadUrl, string downloadCount)
            : base(identity.Id, identity.Version)
        {
            _summary = summary;
            _description = description;
            _authors = authors == null ? new string[0] : authors.ToArray();
            _owners = owners == null ? new string[0] : owners.ToArray();
            _iconUrl = iconUrl;
            _licenseUrl = licenseUrl;
            _projectUrl = projectUrl;
            _reportAbuseUrl = reportAbuseUrl;
            _description = description;
            _summary = summary;
            _tags = tags;
            _dependencies = dependencies;
            _requireLicenseAcceptance = requireLicenseAccept;
            _title = title;
            _downloadUrl = downloadUrl;
            _downloadCount = downloadCount;
            _published = published;
        }

        public string Title
        {
            get
            {
                return _title;
            }
        }

        public string Summary
        {
            get
            {
                return _summary;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public IEnumerable<string> Authors
        {
            get
            {
                return _authors;
            }
        }

        public IEnumerable<string> Owners
        {
            get
            {
                return _owners;
            }
        }

        public string IconUrl
        {
            get
            {
                return _iconUrl;
            }
        }

        public string LicenseUrl
        {
            get
            {
                return _licenseUrl;
            }
        }

        public string ProjectUrl
        {
            get
            {
                return _projectUrl;
            }
        }

        public string DownloadUrl
        {
            get
            {
                return _downloadUrl;
            }
        }

        public string ReportAbuseUrl
        {
            get
            {
                return _reportAbuseUrl;
            }
        }

        public string Tags
        {
            get
            {
                return _tags;
            }
        }

        public string DownloadCount
        {
            get
            {
                return _downloadCount;
            }
        }

        /// <summary>
        /// Parse DownloadCount into an integer
        /// </summary>
        public int DownloadCountAsInt
        {
            get
            {
                int x = 0;
                Int32.TryParse(_downloadCount, out x);
                return x;
            }
        }

        public DateTimeOffset? Published
        {
            get
            {
                return _published;
            }
        }

        /// <summary>
        /// Checks the published date
        /// </summary>
        public bool IsListed
        {
            get
            {
                return Published.HasValue && Published.Value.Year < 1902;
            }
        }

        public string Dependencies
        {
            get
            {
                return _dependencies;
            }
        }

        /// <summary>
        /// Parses Dependencies into actual groups
        /// </summary>
        public IEnumerable<PackageDependencyGroup> DependencySets
        {
            get
            {
                // Ex: Microsoft.Data.OData:5.6.3:aspnetcore50|Microsoft.Data.Services.Client:5.6.3:aspnetcore50|System.Spatial:5.6.3:aspnetcore50|System.Collections:4.0.10-beta-22231:aspnetcore50|System.Collections.Concurrent:4.0.0-beta-22231:aspnetcore50|System.Collections.Specialized:4.0.0-beta-22231:aspnetcore50|System.Diagnostics.Debug:4.0.10-beta-22231:aspnetcore50|System.Diagnostics.Tools:4.0.0-beta-22231:aspnetcore50|System.Diagnostics.TraceSource:4.0.0-beta-22231:aspnetcore50|System.Diagnostics.Tracing:4.0.10-beta-22231:aspnetcore50|System.Dynamic.Runtime:4.0.0-beta-22231:aspnetcore50|System.Globalization:4.0.10-beta-22231:aspnetcore50|System.IO:4.0.10-beta-22231:aspnetcore50|System.IO.FileSystem:4.0.0-beta-22231:aspnetcore50|System.IO.FileSystem.Primitives:4.0.0-beta-22231:aspnetcore50|System.Linq:4.0.0-beta-22231:aspnetcore50|System.Linq.Expressions:4.0.0-beta-22231:aspnetcore50|System.Linq.Queryable:4.0.0-beta-22231:aspnetcore50|System.Net.Http:4.0.0-beta-22231:aspnetcore50|System.Net.Primitives:4.0.10-beta-22231:aspnetcore50|System.Reflection:4.0.10-beta-22231:aspnetcore50|System.Reflection.Extensions:4.0.0-beta-22231:aspnetcore50|System.Reflection.TypeExtensions:4.0.0-beta-22231:aspnetcore50|System.Runtime:4.0.20-beta-22231:aspnetcore50|System.Runtime.Extensions:4.0.10-beta-22231:aspnetcore50|System.Runtime.InteropServices:4.0.20-beta-22231:aspnetcore50|System.Runtime.Serialization.Primitives:4.0.0-beta-22231:aspnetcore50|System.Runtime.Serialization.Xml:4.0.10-beta-22231:aspnetcore50|System.Security.Cryptography.Encoding:4.0.0-beta-22231:aspnetcore50|System.Security.Cryptography.Encryption:4.0.0-beta-22231:aspnetcore50|System.Security.Cryptography.Hashing:4.0.0-beta-22231:aspnetcore50|System.Security.Cryptography.Hashing.Algorithms:4.0.0-beta-22231:aspnetcore50|System.Text.Encoding:4.0.10-beta-22231:aspnetcore50|System.Text.Encoding.Extensions:4.0.10-beta-22231:aspnetcore50|System.Text.RegularExpressions:4.0.10-beta-22231:aspnetcore50|System.Threading:4.0.0-beta-22231:aspnetcore50|System.Threading.Tasks:4.0.10-beta-22231:aspnetcore50|System.Threading.Thread:4.0.0-beta-22231:aspnetcore50|System.Threading.ThreadPool:4.0.10-beta-22231:aspnetcore50|System.Threading.Timer:4.0.0-beta-22231:aspnetcore50|System.Xml.ReaderWriter:4.0.10-beta-22231:aspnetcore50|System.Xml.XDocument:4.0.0-beta-22231:aspnetcore50|System.Xml.XmlSerializer:4.0.0-beta-22231:aspnetcore50|Microsoft.Data.OData:5.6.3:aspnet50|Microsoft.Data.Services.Client:5.6.3:aspnet50|System.Spatial:5.6.3:aspnet50|Microsoft.Data.OData:5.6.2:net40-Client|Newtonsoft.Json:5.0.8:net40-Client|Microsoft.Data.Services.Client:5.6.2:net40-Client|Microsoft.WindowsAzure.ConfigurationManager:1.8.0.0:net40-Client|Microsoft.Data.OData:5.6.2:win80|Microsoft.Data.OData:5.6.2:wpa|Microsoft.Data.OData:5.6.2:wp80|Newtonsoft.Json:5.0.8:wp80

                if (String.IsNullOrEmpty(Dependencies))
                {
                    return Enumerable.Empty<PackageDependencyGroup>();
                }
                else
                {
                    var results = new Dictionary<NuGetFramework, List<PackageDependency>>(new NuGetFrameworkFullComparer());

                    foreach (var set in Dependencies.Split('|'))
                    {
                        string[] parts = set.Split(':');

                        if (parts.Length == 3)
                        {
                            var framework = NuGetFramework.Parse(parts[2]);

                            List<PackageDependency> deps = null;
                            if (!results.TryGetValue(framework, out deps))
                            {
                                deps = new List<PackageDependency>();
                                results.Add(framework, deps);
                            }

                            deps.Add(new PackageDependency(parts[0], VersionRange.Parse(parts[1])));
                        }
                        else
                        {
                            Debug.Fail("Unknown dependency format: " + set);
                        }
                    }

                    return results.Select(pair => new PackageDependencyGroup(pair.Key, pair.Value));
                }
            }
        }

        public bool RequireLicenseAcceptance
        {
            get
            {
                return _requireLicenseAcceptance;
            }
        }
    }
}