using System.ServiceModel;
using Marvin.Tools.Wcf;
using Marvin.Tools.Wcf.FileSystem;

namespace Marvin.Runtime.Maintenance.Filesystem
{
    /// <summary>
    /// Service contract for the maintenance file system.
    /// </summary>
    [ServiceContract]
    [ServiceVersion(ServerVersion = "1.0.0.0", MinClientVersion = "1.0.0.0")]
    public interface IMaintenanceFileSystemService : IFileSystemService
    {
    }
}
