using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.Modules;

namespace Marvin.Runtime.UserManagement.ClientConfigStore
{
    /// <summary>
    /// Config store in the file system
    /// </summary>
    [DataContract]
    public class FileSystemClientConfigStoreConfig : ClientConfigStoreConfigBase
    {
        /// <see cref="IPluginConfig.PluginName"/>
        public override string PluginName
        {
            get { return FileSystemClientConfigStore.ComponentName; }
        }

        /// <summary>
        /// Constructor of the configuration
        /// </summary>
        public FileSystemClientConfigStoreConfig()
        {
            ConfigFolder = @".\Config\Client";
        }

        /// <summary>
        /// Path where client configurations should be stored
        /// </summary>
        [DataMember]
        [DefaultValue(@".\Config\Client")]
        public string ConfigFolder { get; set; }

    }
}