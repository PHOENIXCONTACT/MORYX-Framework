using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.AbstractionLayer.Resources;
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
        /// Creates a new instance of the module config
        /// </summary>
        public ModuleConfig()
        {
            DefaultResource = nameof(ResourceInteractionHost);
        }

        /// <summary>
        /// If database is empty, this resource will be created by default.
        /// </summary>
        [DataMember, DefaultValue(nameof(ResourceInteractionHost))]
        [Description("If database is empty, this resource will be created by default.")]
        [PluginNameSelector(typeof(IDefaultResource))]
        public string DefaultResource { get; set; }
    }
}
