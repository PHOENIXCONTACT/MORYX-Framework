using Marvin.Container;
using Marvin.Modules.ModulePlugins;
using Marvin.Runtime.Maintenance.Contracts;
using Marvin.Runtime.Maintenance.Plugins.ModuleMaintenance.Wcf;

namespace Marvin.Runtime.Maintenance.Plugins.ModuleMaintenance
{
    [ExpectedConfig(typeof(ModuleMaintenanceConfig))]
    [Plugin(LifeCycle.Singleton, typeof(IMaintenancePlugin), Name = PluginName)]
    internal class ModuleMaintenancePlugin : MaintenancePluginBase<ModuleMaintenanceConfig, IModuleMaintenance>
    {
        public const string PluginName = "ModuleMaintenance";
    }
}
