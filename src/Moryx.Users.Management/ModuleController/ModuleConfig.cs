using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Configuration;

namespace Moryx.Users.Management
{
    /// <summary>
    /// Module configuration of the <see cref="ModuleController"/>
    /// </summary>
    [DataContract]
    public class ModuleConfig : ConfigBase
    {
        /// <summary>
        /// Configurable card number to identify the default user
        /// </summary>
        [DataMember, Description("Cardnumber of the default user")]
        [DefaultValue("9999")]
        public string DefaultUser { get; set; }
    }
}