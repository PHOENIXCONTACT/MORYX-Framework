using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.Runtime.Configuration;
using Marvin.Runtime.Maintenance.Contracts;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.Maintenance.Plugins.DatabaseMaintenance
{
    /// <summary>
    /// Configuration for the database maintenance plugin.
    /// </summary>
    [DataContract]
    public class DatabaseConfig : MaintenancePluginConfig
    {
        /// <summary>
        /// Provide an endpoint named "DatabaseMaintenance" with binding type "BasicHttp".
        /// </summary>
        public DatabaseConfig()
        {
            ProvidedEndpoint = new HostConfig
            {
                BindingType = ServiceBindingType.BasicHttp,
                Endpoint = "DatabaseMaintenance",
                MetadataEnabled = true
            };
        }

        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public override string PluginName => DatabasePlugin.ComponentName;

        /// <summary>
        /// Folder where the setup data is stored. 
        /// </summary>
        [DataMember]
        [RelativeDirectories]
        [DefaultValue(@".\Backups\")]
        public string SetupDataDir { get; set; }
    }
}
