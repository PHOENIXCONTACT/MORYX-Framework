using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.AbstractionLayer.Resources;
using Marvin.Configuration;
using Marvin.Resources.Interaction;
using Marvin.Runtime.Configuration;

namespace Marvin.Resources.Management
{
    /// <summary>
    /// Configuration of this module
    /// </summary>
    [DataContract]
    public class ModuleConfig : ConfigBase
    {
        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            Initializers = new List<ResourceInitializerConfig>
            {
                new ResourceInitializerConfig
                {
                    PluginName = nameof(ResourceInteractionInitializer)
                }
            };
        }

        /// <summary>
        /// If database is empty, this resource will be created by default.
        /// </summary>
        [DataMember, Description("Configuration of possible resource initializer")]
        [PluginConfigs(typeof(IResourceInitializer), true)]
        public List<ResourceInitializerConfig> Initializers { get; set; }
    }
}
