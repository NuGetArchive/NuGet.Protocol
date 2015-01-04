using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using NuGet.Client;
using NuGet.Client.VisualStudio.Models;
using System.Diagnostics;
using System.Runtime.Versioning;
using NuGet.Versioning;
using Newtonsoft.Json.Linq;
using Xunit;

namespace V2V3ResourcesTest
{
    public class V3ResourceTest
    {
        private CompositionContainer container;
        private string V3SourceUrl = "https://az320820.vo.msecnd.net/ver3-preview/index.json";
        public V3ResourceTest()
        {
            try
            {
                //Creating an instance of aggregate catalog. It aggregates other catalogs
                var aggregateCatalog = new AggregateCatalog();
                //Build the directory path where the parts will be available
                var directoryPath = Environment.CurrentDirectory;
                var directoryCatalog = new DirectoryCatalog(directoryPath, "*V3*.dll");
                aggregateCatalog.Catalogs.Add(directoryCatalog);
                container = new CompositionContainer(aggregateCatalog);
                container.ComposeParts(this);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Fact]
        public void TestV3DownloadResource()
        {
            IEnumerable<Lazy<ResourceProvider, IResourceProviderMetadata>> providers = container.GetExports<ResourceProvider, IResourceProviderMetadata>();
            Assert.True(providers.Count() > 0);    
            PackageSource source = new PackageSource("V3Source", V3SourceUrl);
            SourceRepository2 repo = new SourceRepository2(source,providers);
            IDownload resource = (IDownload)repo.GetResource<IDownload>().Result;
            Assert.True(resource != null);
            Assert.True(resource.GetType().GetInterfaces().Contains(typeof(IDownload)));
            PackageDownloadMetadata downloadMetadata = resource.GetNupkgUrlForDownload(new PackageIdentity("jQuery", new NuGetVersion("1.6.4"))).Result;
            //*TODOs: Check if the download Url ends with .nupkg. More detailed verification can be added to see if the nupkg file can be fetched from the location.
            Assert.True(downloadMetadata.NupkgDownloadUrl.OriginalString.EndsWith(".nupkg")); 
        }

        [Fact]
        public void TestV3MetadataResource()
        {
            IEnumerable<Lazy<ResourceProvider, IResourceProviderMetadata>> providers = container.GetExports<ResourceProvider, IResourceProviderMetadata>();
            Assert.True(providers.Count() > 0);         
            PackageSource source = new PackageSource("V3Source", V3SourceUrl);
            SourceRepository2 repo = new SourceRepository2(source,providers);
            IMetadata resource = (IMetadata)repo.GetResource<IMetadata>().Result;
            Assert.True(resource != null);
            Assert.True(resource.GetType().GetInterfaces().Contains(typeof(IMetadata)));
            NuGetVersion latestVersion = resource.GetLatestVersion("jQuery").Result;
            //*TODOs: Use a proper test package whose latest version is fixed instead of using jQuery.
            Assert.True(latestVersion.ToNormalizedString().Equals("2.1.1")); 
        }

        [Fact]
        public void TestV3VisualStudioUIMetadataResource()
        {
            IEnumerable<Lazy<ResourceProvider, IResourceProviderMetadata>> providers = container.GetExports<ResourceProvider, IResourceProviderMetadata>();
            Assert.True(providers.Count() > 0);
            PackageSource source = new PackageSource("V3Source", V3SourceUrl);
            SourceRepository2 repo = new SourceRepository2(source, providers);
            IVisualStudioUIMetadata resource = (IVisualStudioUIMetadata)repo.GetResource<IVisualStudioUIMetadata>().Result;
            Assert.True(resource != null);
            Assert.True(resource.GetType().GetInterfaces().Contains(typeof(IVisualStudioUIMetadata)));
            //*TODOs: Use a proper test package whose metatdata and versions are fixed instead of exisitng packages.
            VisualStudioUIPackageMetadata packageMetadata = resource.GetPackageMetadataForVisualStudioUI("Microsoft.AspNet.Razor", new NuGetVersion("4.0.0-beta1")).Result;
            Assert.True(packageMetadata.HasDependencies.Equals(true)); 
            Assert.True(packageMetadata.DependencySets.Count() == 1);
            Assert.True(packageMetadata.DependencySets.First().Dependencies.Count().Equals(12));
            
        }

        [Fact]
        public void TestV3VisualStudioUISearchResource()
        {
            IEnumerable<Lazy<ResourceProvider, IResourceProviderMetadata>> providers = container.GetExports<ResourceProvider, IResourceProviderMetadata>();
            Assert.True(providers.Count() > 0);
            PackageSource source = new PackageSource("V3Source", V3SourceUrl);
            SourceRepository2 repo = new SourceRepository2(source,providers);
            IVisualStudioUISearch resource = (IVisualStudioUISearch)repo.GetResource<IVisualStudioUISearch>().Result;
            //Check if we are able to obtain a resource
            Assert.True(resource != null);
            //check if the resource is of type IVsSearch.
            Assert.True(resource.GetType().GetInterfaces().Contains(typeof(IVisualStudioUISearch))); 
            SearchFilter filter = new SearchFilter(); //create a dummy filter.
            List<string> fxNames = new List<string>();
            fxNames.Add(new FrameworkName(".NET Framework, Version=4.0").FullName);
            filter.SupportedFrameworks = fxNames;
            IEnumerable<VisualStudioUISearchMetadata> searchResults = resource.GetSearchResultsForVisualStudioUI("Elmah", filter, 0, 100, new System.Threading.CancellationToken()).Result;
            // Check if non empty search result is returned.
            Assert.True(searchResults.Count() > 0);
            //check if there is atleast one result which has Elmah as title.
            Assert.True(searchResults.Any(p => p.Id.Equals("Elmah", StringComparison.OrdinalIgnoreCase))); 
        }

        [Fact]
        public void TestV3PowerShellAutocompleteResourceForPackageIds()
        {
            IEnumerable<Lazy<ResourceProvider, IResourceProviderMetadata>> providers = container.GetExports<ResourceProvider, IResourceProviderMetadata>();
            Assert.True(providers.Count() > 0);
            PackageSource source = new PackageSource("V3Source", V3SourceUrl);
            SourceRepository2 repo = new SourceRepository2(source, providers);
            IPowerShellAutoComplete resource = (IPowerShellAutoComplete)repo.GetResource<IPowerShellAutoComplete>().Result;
            //Check if we are able to obtain a resource
            Assert.True(resource != null); 
            Assert.True(resource.GetType().GetInterfaces().Contains(typeof(IPowerShellAutoComplete))); //check if the resource is of type IVsSearch.     
            IEnumerable<string> searchResults = resource.GetPackageIdsStartingWith("Elmah", new System.Threading.CancellationToken()).Result;
            // Check if non empty search result is returned.
            Assert.True(searchResults.Count() > 0); 
            //Make sure that all the package ids contains the search term in it.
            Assert.True(!searchResults.Any(p => p.IndexOf("Elmah",StringComparison.OrdinalIgnoreCase) == -1)); 
        }

        [Fact]
        public void TestV3PowerShellAutocompleteResourceForPackageVersions()
        {
            IEnumerable<Lazy<ResourceProvider, IResourceProviderMetadata>> providers = container.GetExports<ResourceProvider, IResourceProviderMetadata>();
            Assert.True(providers.Count() > 0);
            PackageSource source = new PackageSource("V3Source", V3SourceUrl);
            SourceRepository2 repo = new SourceRepository2(source, providers);
            IPowerShellAutoComplete resource = (IPowerShellAutoComplete)repo.GetResource<IPowerShellAutoComplete>().Result;
            //Check if we are able to obtain a resource
            Assert.True(resource != null);
            Assert.True(resource.GetType().GetInterfaces().Contains(typeof(IPowerShellAutoComplete))); //check if the resource is of type IVsSearch.     
            // Check if non zero version count is returned. *TODOS : Use a standard test packages whose version count will be fixed
            IEnumerable<NuGetVersion> versions = resource.GetAllVersions("jQuery").Result ;            
            Assert.True(versions.Count() >= 35);
        }   
    }
}
