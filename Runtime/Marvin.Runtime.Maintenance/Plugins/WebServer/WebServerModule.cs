using Marvin.Container;
using Marvin.Modules.ModulePlugins;
using Marvin.Runtime.Maintenance.Contracts;
using Marvin.Runtime.Maintenance.Filesystem;

namespace Marvin.Runtime.Maintenance.Plugins.WebServer
{
    [ExpectedConfig(typeof(WebServerConfig))]
    [Plugin(LifeCycle.Singleton, typeof(IMaintenancePlugin), Name = PluginName)]
    internal class WebServerPlugin : MaintenancePluginBase<WebServerConfig, IMaintenanceFileSystemService>
    {
        public const string PluginName = "WebServerModule";

        public FileSystemPathProvider PathProvider { get; set; }

        public override void Initialize(MaintenancePluginConfig config)
        {
            base.Initialize(config);
            PathProvider.LoadPaths();
        }
    }
}
