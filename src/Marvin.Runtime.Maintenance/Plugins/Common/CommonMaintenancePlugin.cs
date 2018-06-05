using Marvin.Container;
using Marvin.Modules;
using Marvin.Runtime.Maintenance.Contracts;

namespace Marvin.Runtime.Maintenance.Plugins.Common
{
    /// <summary>
    /// Common maintenace plugin.
    /// </summary>
    [ExpectedConfig(typeof(CommonMaintenanceConfig))]
    [Plugin(LifeCycle.Singleton, typeof(IMaintenancePlugin), Name = PluginName)]
    public class CommonMaintenancePlugin : MaintenancePluginBase<CommonMaintenanceConfig, ICommonMaintenance>
    {
        internal const string PluginName = "CommonMaintenance";
    }
}
