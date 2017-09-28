using Marvin.Container;
using Marvin.Modules.ModulePlugins;
using Marvin.Runtime.Maintenance.Contracts;
using Marvin.Runtime.Maintenance.Filesystem;

namespace Marvin.Runtime.Maintenance.Plugins.DataStore
{
    [ExpectedConfig(typeof(DataStoreConfig))]
    [Plugin(LifeCycle.Singleton, typeof(IMaintenancePlugin), Name = PluginName)]
    internal class DataStorePlugin : MaintenancePluginBase<DataStoreConfig, IMaintenanceFileSystemService>
    {
        public const string PluginName = "DataStorePlugin";

        // Injected properties
        public FileSystemPathProvider PathProvider { get; set; }

        public override void Initialize(MaintenancePluginConfig config)
        {
            base.Initialize(config);
            PathProvider.DataStoreRoot = Config.DataStoreRoot;
            PathProvider.LoadPaths();
        }
    }
}
