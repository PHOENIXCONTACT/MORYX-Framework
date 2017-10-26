using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.Configuration;
using Marvin.Runtime.Configuration;
using Marvin.Runtime.UserManagement.ClientConfigStore;
using Marvin.Runtime.UserManagement.UserAuthenticator;
using Marvin.Runtime.UserManagement.UserGroupProvider;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.UserManagement
{
    /// <summary>
    /// Configuration for the user manager.
    /// </summary>
    [DataContract]
    public class ModuleConfig : ConfigBase
    {
        /// <summary>
        /// Constructor for the UserManagerConfig.
        /// </summary>
        public ModuleConfig()
        {
            HostConfig = new HostConfig
            {
                Endpoint = "UserManagement",
                BindingType = ServiceBindingType.BasicHttp,
                RequiresAuthentification = true,
                MetadataEnabled = true,
                HelpEnabled = true
            };

            ConfigStorage = new FileSystemClientConfigStoreConfig();
        }

        /// <summary>
        /// Flag if this config was allready initialized.
        /// </summary>
        [DataMember]
        [Description("Flag if this config was allready initialized.")]
        public bool ConfigInitialized { get; set; }
        
        /// <summary>
        /// Wcf endpoint hosting the UserManagement service.
        /// </summary>
        [DataMember]
        [Description("Wcf endpoint hosting the UserManagement service.")]
        public HostConfig HostConfig { get; set; }

        /// <summary>
        /// List of <see cref="UserAuthenticatorConfigBase"/>.
        /// </summary>
        [DataMember]
        [Description("List of all active application authenticators.")]
        [PluginConfigs(typeof(IUserAuthenticator), false)]
        public List<UserAuthenticatorConfigBase> AuthenticatorConfigs { get; set; }

        /// <summary>
        /// List of <see cref="UserGroupProviderConfigBase"/>.
        /// </summary>
        [DataMember]
        [Description("List of all active providers used to determine the users groups")]
        [PluginConfigs(typeof(IUserGroupProvider))]
        public List<UserGroupProviderConfigBase> UserGroupProviders { get; set; }

        /// <summary>
        /// Strategy to select the storage type of client based configs
        /// </summary>
        [DataMember]
        [Description("Strategy to select the storage type of client based configs")]
        [PluginConfigs(typeof(IClientConfigStore), false)]
        public ClientConfigStoreConfigBase ConfigStorage { get; set; } 
    }
}
