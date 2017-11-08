using Marvin.Container;
using Marvin.Modules;
using Marvin.Runtime.Maintenance.Contracts;
using Marvin.Runtime.Maintenance.Plugins.CommonMaintenance.Wcf;

namespace Marvin.Runtime.Maintenance.Plugins.CommonMaintenance
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
