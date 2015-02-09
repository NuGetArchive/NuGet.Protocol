using System;
using NuGet.Versioning;

namespace NuGet.Client
{
    public class V3ReportAbuseResource : INuGetResource
    {
        Uri _reportAbuseTemplate;

        public V3ReportAbuseResource(Uri reportAbuseTemplate)
        {
            _reportAbuseTemplate = reportAbuseTemplate;
        }

        public Uri GetReportAbuseUrl(string id, NuGetVersion version)
        {
            return NuGet.Data.Utility.ApplyPackageIdVersionToUriTemplate(_reportAbuseTemplate, id, version);
        }
    }
}
