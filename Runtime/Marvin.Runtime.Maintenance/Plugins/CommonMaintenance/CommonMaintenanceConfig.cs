using System.Runtime.Serialization;
using Marvin.Runtime.Maintenance.Contracts;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.Maintenance.Plugins.CommonMaintenance
{
    /// <summary>
    /// Configuration for the common maintenance plugin.
    /// </summary>
    [DataContract]
    public class CommonMaintenanceConfig : MaintenancePluginConfig
    {
        /// <summary>
        /// The name of the plugin.
        /// </summary>
        public override string PluginName => CommonMaintenancePlugin.PluginName;

        /// <summary>
        /// Constructor for the common maintenance config. Creates an "CommonMaintenance" endpoint with binding type "BasicHttp".
        /// </summary>
        public CommonMaintenanceConfig()
        {
            ProvidedEndpoint = new HostConfig()
            {
                Endpoint = "CommonMaintenance",
                BindingType = ServiceBindingType.BasicHttp,
            };
        }
    }
}
