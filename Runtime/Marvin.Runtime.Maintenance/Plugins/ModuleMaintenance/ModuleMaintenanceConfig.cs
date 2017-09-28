using System.Runtime.Serialization;
using Marvin.Runtime.Maintenance.Contracts;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.Maintenance.Plugins.ModuleMaintenance
{
    [DataContract]
    internal class ModuleMaintenanceConfig : MaintenancePluginConfig
    {
        public ModuleMaintenanceConfig()
        {
            ProvidedEndpoint = new HostConfig
            {
                BindingType = ServiceBindingType.BasicHttp,
                Endpoint = "ModuleMaintenance",
                MetadataEnabled = true
            };
        }

        public override string PluginName
        {
            get { return ModuleMaintenancePlugin.PluginName; }
        }
    }
}
