using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.Configuration;
using Marvin.Runtime.Configuration;

namespace Marvin.Resources.Management
{
    /// <summary>
    /// Configuration of this module
    /// </summary>
    [DataContract]
    public class ModuleConfig : ConfigBase
    {
        /// <summary>
        /// Resource type of the root resource
        /// </summary>
        [DataMember, DefaultValue(nameof(RootResource))]
        [PluginNameSelector(typeof(IRootResource)), ModuleStrategy(typeof(IRootResource))]
        public string RootType { get; set; }
    }
}
