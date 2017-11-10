using System.Diagnostics;
using System.IO;
using Marvin.Container;

namespace Marvin.Runtime.Maintenance.Filesystem
{
    /// <summary>
    /// Provides the pathes to the root of important functions.
    /// </summary>
    [Plugin(LifeCycle.Singleton)]
    public class FileSystemPathProvider
    {
        /// <summary>
        /// Load the root pathes.
        /// </summary>
        public void LoadPaths()
        {
            var processFileInfo = new FileInfo(Process.GetCurrentProcess().MainModule.FileName);

            SilverlightAppWebRoot = $"{processFileInfo.DirectoryName}\\EddieLight\\SilverlightApp\\";
            AppWebRoot = $"{processFileInfo.DirectoryName}\\EddieLight\\Maintenance\\";
            DataStoreRoot = Path.Combine(processFileInfo.DirectoryName, string.IsNullOrEmpty(DataStoreRoot) ? "Backups" : DataStoreRoot);
        }

        /// <summary>
        /// Root path for the silverlight web app.
        /// </summary>
        public string SilverlightAppWebRoot { get; set; }

        /// <summary>
        /// Root path for the web app.
        /// </summary>
        public string AppWebRoot { get; set; }

        /// <summary>
        /// Root path for the data store.
        /// </summary>
        public string DataStoreRoot { get; set; }
    }
}
