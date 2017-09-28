using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Marvin.Runtime.UserManagement.UserGroupProvider
{
    /// <summary>
    /// Configurates a group and his members.
    /// </summary>
    [DataContract]
    public class ConfigGroupProviderConfig : UserGroupProviderConfigBase
    {
        /// <summary>
        /// Name of the plugin
        /// </summary>
        public override string PluginName { get { return ConfigGroupProvider.ComponentName; } set { } }

        /// <summary>
        /// List of users of this group.
        /// </summary>
        [DataMember]
        public List<SimpleUserModel> Users { get; set; } 
    }

    /// <summary>
    /// Simple configuration model for a user.
    /// </summary>
    [DataContract]
    public class SimpleUserModel
    {
        /// <summary>
        /// Name of the user.
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// List of groups where the user belongs to.
        /// </summary>
        [DataMember]
        public List<SimpleGroupModel> Groups { get; set; } 

        /// <summary>
        /// Format: "Username": "Groups.Count" groups.
        /// </summary>
        /// <returns>Format: "Username": "Groups.Count" groups.</returns>
        public override string ToString()
        {
            return string.Format("{0}: {1} groups", UserName, Groups.Count);
        }
    }

    /// <summary>
    /// Model of a simple group.
    /// </summary>
    [DataContract]
    public class SimpleGroupModel
    {
        /// <summary>
        /// Name of the group.
        /// </summary>
        [DataMember]
        public string GroupName { get; set; }
    }
}
