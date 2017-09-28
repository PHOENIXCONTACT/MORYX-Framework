using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.Runtime.Maintenance.Contracts;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.Maintenance.Plugins.DataStore
{
    [DataContract]
    class DataStoreConfig : MaintenancePluginConfig
    {
        public DataStoreConfig()
        {
            ProvidedEndpoint = new HostConfig
            {
                Endpoint = "MaintenanceFileSystem",
                BindingType = ServiceBindingType.BasicHttp,
            };
        }

        public override string PluginName => DataStorePlugin.PluginName;

        [DataMember]
        [Description("Directory displayed as root the data store plugin.")]
        [DefaultValue(@".\Backups\")]
        public string DataStoreRoot { get; set; }
    }
}
