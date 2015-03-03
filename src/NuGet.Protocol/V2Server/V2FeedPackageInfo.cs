using NuGet.Packaging;
using NuGet.Packaging.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NuGet.Protocol
{
    public class V2FeedPackageInfo : PackageIdentity
    {
        private readonly string _title;
        private readonly string _summary;
        private readonly string _authors;
        private readonly string _description;
        private readonly string _owners;
        private readonly string _iconUrl;
        private readonly string _licenseUrl;
        private readonly string _projectUrl;
        private readonly string _reportAbuseUrl;
        private readonly string _tags;
        private readonly int _downloadCount;
        private readonly bool _requireLicenseAcceptance;
        private readonly DateTimeOffset? _published;
        private readonly PackageDependencyGroup[] _dependencySets;
        private readonly string _downloadUrl;

        public V2FeedPackageInfo(PackageIdentity identity, string title, string summary, string description, string authors, string owners,
            string iconUrl, string licenseUrl, string projectUrl, string reportAbuseUrl,
            string tags, DateTimeOffset? published, IEnumerable<PackageDependencyGroup> dependencySet, bool requireLicenseAccept, string downloadUrl, int downloadCount)
            : base(identity.Id, identity.Version)
        {
            if (dependencySet == null)
            {
                throw new ArgumentNullException("dependencySet");
            }

            _summary = summary;
            _description = description;
            _authors = authors;
            _owners = owners;
            _iconUrl = iconUrl;
            _licenseUrl = licenseUrl;
            _projectUrl = projectUrl;
            _reportAbuseUrl = reportAbuseUrl;
            _description = description;
            _summary = summary;
            _tags = tags;
            _dependencySets = dependencySet.ToArray();
            _requireLicenseAcceptance = requireLicenseAccept;
            _title = title;
            _downloadUrl = downloadUrl;
            _downloadCount = downloadCount;
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

        public string Authors
        {
            get
            {
                return _authors;
            }
        }

        public string Owners
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

        public int DownloadCount
        {
            get
            {
                return _downloadCount;
            }
        }

        public DateTimeOffset? Published
        {
            get
            {
                return _published;
            }
        }

        public IEnumerable<PackageDependencyGroup> DependencySets
        {
            get
            {
                return _dependencySets;
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