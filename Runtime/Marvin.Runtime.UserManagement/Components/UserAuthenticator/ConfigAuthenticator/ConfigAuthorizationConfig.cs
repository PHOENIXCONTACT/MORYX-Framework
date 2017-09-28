using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Marvin.Runtime.UserManagement.UserAuthenticator
{
    /// <summary>
    /// Configuration for the authorization config.
    /// </summary>
    [DataContract]
    public class ConfigAuthorizationConfig : UserAuthenticatorConfigBase
    {
        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public override string PluginName
        {
            get { return ConfigAuthenticator.ComponentName; }
        }

        /// <summary>
        /// configurates the authenticationstoretype. See <see cref="UserAuthenticator.AuthenticationStore"/> for detail.
        /// </summary>
        public override AuthenticationStore AuthenticationStoreType
        {
            get { return UserAuthenticator.AuthenticationStore.Config; }
        }

        /// <summary>
        /// Name of the authenticationstore.
        /// </summary>
        public override string AuthenticationStore
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// List of access configurations.
        /// </summary>
        [DataMember]
        public List<AccessConfiguration> AccessConfigurations { get; set; } 
    }

    /// <summary>
    /// Configuration of the accesses.
    /// </summary>
    [DataContract]
    public class AccessConfiguration
    {
        /// <summary>
        /// Group where the user belongs to.
        /// </summary>
        [DataMember]
        [DefaultValue(ConfigAuthenticator.CommonGroup)]
        public string UserGroup { get; set; }

        /// <summary>
        /// configuration for the application.
        /// </summary>
        [DataMember]
        public ApplicationConfig Configuration { get; set; }

        /// <summary>
        /// List of operation accesses. Gives information about what the user in this group can do.
        /// </summary>
        [DataMember]
        public List<OperationAccessPair> OperationAccesses { get; set; }

        /// <summary>
        /// Format: "UserGroup": "PluginCount" plugins and "OperationAccessCounts" operations. 
        /// </summary>
        /// <returns>Format: "UserGroup": "PluginCount" plugins and "OperationAccessCounts" operations. </returns>
        public override string ToString()
        {
            return string.Format("{0}: {1} plugins and {2} operations", 
                                 UserGroup, Configuration.PluginConfigs.Count, OperationAccesses.Count);
        }
    }

    /// <summary>
    /// Configuration for the application. It describes the application for which the access should be defined.
    /// </summary>
    [DataContract]
    public class ApplicationConfig
    {
        /// <summary>
        /// Constructor for the application configuration.
        /// </summary>
        public ApplicationConfig()
        {
            PluginConfigs = new List<PluginConfig>();
        }
        /// <summary>
        /// Name of the application.
        /// </summary>
        [DataMember]
        public string Application { get; set; }

        /// <summary>
        /// <see cref="PluginConfig"/> for the shell dll.
        /// </summary>
        [DataMember]
        public PluginConfig ShellDll { get; set; }

        /// <summary>
        /// List of sub plugin configs.
        /// </summary>
        [DataMember]
        public List<PluginConfig> PluginConfigs { get; set; }
    }

    /// <summary>
    /// Describes a plugin for the access rights.
    /// </summary>
    [DataContract]
    public class PluginConfig
    {
        /// <summary>
        /// Name of the application.
        /// </summary>
        [DataMember]
        public string Application { get; set; }

        /// <summary>
        /// Name of the plugin dll.
        /// </summary>
        [DataMember]
        public string PluginDll { get; set; }

        /// <summary>
        /// List of dependecies of the plugin.
        /// </summary>
        [DataMember]
        public List<Dependency> Dependencies { get; set; }

        /// <summary>
        /// The sort index.
        /// </summary>
        [DataMember]
        public int SortIndex { get; set; }

        /// <summary>
        /// The overriden display name.
        /// </summary>
        [DataMember]
        public string OverrideDisplayName { get; set; }

        /// <summary>
        /// Override ToString(): Format: "SortIndex": "PluginDll".
        /// </summary>
        /// <returns> Format: "SortIndex": "PluginDll".</returns>
        public override string ToString()
        {
            return string.Format("{0}: {1}", SortIndex, PluginDll);
        }
    }

    /// <summary>
    /// Describes a dependency.
    /// </summary>
    [DataContract]
    public class Dependency
    {
        /// <summary>
        /// Name of the library.
        /// </summary>
        [DataMember]
        public string LibraryName { get; set; }

        /// <summary>
        /// Override ToString(): Format: "LibraryName"
        /// </summary>
        /// <returns>Format: "LibraryName"</returns>
        public override string ToString()
        {
            return LibraryName;
        }
    }

    /// <summary>
    /// Describes an operation and the access rights for it.
    /// </summary>
    [DataContract]
    public class OperationAccessPair
    {
        /// <summary>
        /// An operation.
        /// </summary>
        [DataMember]
        public string Operation { get; set; }

        /// <summary>
        /// The access rights to the operation. See <see cref="OperationAccess"/> for details.
        /// </summary>
        [DataMember]
        public OperationAccess AccessRight { get; set; }

        /// <summary>
        /// Overrides ToString(): Format: "Operation": "AccessRight".
        /// </summary>
        /// <returns>Format: "Operation": "AccessRight".</returns>
        public override string ToString()
        {
            return string.Format("{0}: {1}", Operation, AccessRight);
        }
    }
}
