using System.ServiceModel;
using Marvin.Container;
using Marvin.Tools.Wcf.FileSystem;

namespace Marvin.Runtime.Maintenance.Filesystem
{
    /// <summary>
    /// Service for the file system. 
    /// </summary>
    [Plugin(LifeCycle.Transient, typeof(IMaintenanceFileSystemService))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, IncludeExceptionDetailInFaults = true)]
    public class MaintenanceFileSystemService :  FileSystemService, IMaintenanceFileSystemService
    {
        /// <summary>
        /// Constructor for the maintenance file system service.
        /// </summary>
        /// <param name="provider">File system provider with the necessary information about the root pathes.</param>
        public MaintenanceFileSystemService(FileSystemPathProvider provider)
        {
            WebRoot = provider.SilverlightAppWebRoot;
            Root = provider.DataStoreRoot;

            SecondaryWebRoots.Add(provider.AppWebRoot);
            SecondaryWebRoots.Add(Root);
        }
    }
}
