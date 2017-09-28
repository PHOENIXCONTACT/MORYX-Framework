using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.Configuration;
using Marvin.Runtime.Configuration;
using Marvin.Runtime.Maintenance.Contracts;
using Marvin.Runtime.Maintenance.Plugins.CommonMaintenance;
using Marvin.Runtime.Maintenance.Plugins.DatabaseMaintenance;
using Marvin.Runtime.Maintenance.Plugins.DataStore;
using Marvin.Runtime.Maintenance.Plugins.LogMaintenance;
using Marvin.Runtime.Maintenance.Plugins.ModuleMaintenance;
using Marvin.Runtime.Maintenance.Plugins.WebServer;

namespace Marvin.Runtime.Maintenance
{
    /// <summary>
    /// Configuration of the maintenance module.
    /// </summary>
    [DataContract]
    public class ModuleConfig : ConfigBase
    {
        /// <summary>
        /// List of configured maintenance modules.
        /// </summary>
        [DataMember]
        [PluginConfigs(typeof(IMaintenancePlugin), false)]
        [Description("List of configured maintenance modules.")]
        public MaintenancePluginConfig[] Plugins { get; set; }

        /// <summary>
        /// Initialize the maintenance module.
        /// </summary>
        protected override void Initialize()
        {
            Plugins = new MaintenancePluginConfig[]
            {
                new WebServerConfig {IsActive = true},
                new ModuleMaintenanceConfig {IsActive = true},
                new LoggingMaintenanceConfig {IsActive = true},
                new DataStoreConfig {IsActive = true},
                new CommonMaintenanceConfig {IsActive = true},
                new DatabaseConfig {IsActive = true}
            };
        }
    }
}
