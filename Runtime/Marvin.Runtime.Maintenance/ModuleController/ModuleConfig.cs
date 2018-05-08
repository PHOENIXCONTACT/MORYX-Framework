using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.Configuration;
using Marvin.Runtime.Maintenance.Contracts;
using Marvin.Runtime.Maintenance.Plugins.Common;
using Marvin.Runtime.Maintenance.Plugins.Databases;
using Marvin.Runtime.Maintenance.Plugins.Logging;
using Marvin.Runtime.Maintenance.Plugins.Modules;

namespace Marvin.Runtime.Maintenance
{
    /// <summary>
    /// Configuration of the maintenance module.
    /// </summary>
    [DataContract]
    public class ModuleConfig : ConfigBase
    {
        /// <inheritdoc />
        protected override bool PersistDefaultConfig => true;

        /// <summary>
        /// List of configured maintenance modules.
        /// </summary>
        [DataMember]
        [PluginConfigs(typeof(IMaintenancePlugin), false)]
        [Description("List of configured maintenance modules.")]
        public List<MaintenancePluginConfig> Plugins { get; set; }

        /// <summary>
        /// Initialize the maintenance module.
        /// </summary>
        protected override void Initialize()
        {
            Plugins = new List<MaintenancePluginConfig>
            {
                new ModuleMaintenanceConfig(),
                new LoggingMaintenanceConfig(),
                new CommonMaintenanceConfig(),
                new DatabaseConfig()
            };
        }
    }
}
