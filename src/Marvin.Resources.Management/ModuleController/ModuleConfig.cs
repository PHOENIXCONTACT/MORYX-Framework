using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.AbstractionLayer.Resources;
using Marvin.Configuration;
using Marvin.Serialization;

namespace Marvin.Resources.Management
{
    /// <summary>
    /// Configuration of this module
    /// </summary>
    [DataContract]
    public class ModuleConfig : ConfigBase
    {
        /// <summary>
        /// If database is empty, this resource will be created by default.
        /// </summary>
        [DataMember, Description("Configuration of possible resource initializer")]
        [PluginConfigs(typeof(IResourceInitializer), true)]
        public List<ResourceInitializerConfig> Initializers { get; set; }
    }
}
