using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.Modules;
using Marvin.Runtime.Configuration;

namespace Marvin.Runtime.UserManagement.UserGroupProvider
{
    /// <summary>
    /// Configuration for the user group provider.
    /// </summary>
    [DataContract]
    public class UserGroupProviderConfigBase : IPluginConfig
    {
        /// <summary>
        /// Name of the default provider.
        /// </summary>
        public const string DefaultProvider = "LdapGroupProvider";

        /// <summary>
        /// Name of the plugin.
        /// </summary>
        [DataMember]
        [PluginNameSelector(typeof(IUserGroupProvider))]
        public virtual string PluginName { get; set; }

        /// <summary>
        /// Flag if group provider participates in the group resolution process. Does not support hot plugging!
        /// </summary>
        [DataMember]
        [Description("Flag if group provider participates in the group resolution process. Does not support hot plugging!")]
        public bool Active { get; set; }

        /// <summary>
        /// Format: "PluginName"
        /// </summary>
        /// <returns>returns the PluginName.</returns>
        public override string ToString()
        {
            return PluginName;
        }
    }
}