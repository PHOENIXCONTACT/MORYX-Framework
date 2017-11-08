using System.Collections.Generic;
using System.Runtime.Serialization;
using Marvin.Modules;
using Marvin.Runtime.Configuration;

namespace Marvin.TestModule
{
    [DataContract]
    public class AnotherPluginConfig : IPluginConfig
    {
        /// <summary>
        /// Name of the component represented by this entry
        /// </summary>
        [DataMember]
        public string PluginName { get; set; }
    }

    [DataContract]
    public class AnotherPluginConfig2 : AnotherPluginConfig
    {
        [DataMember]
        [PluginConfigs(typeof(IAnotherSubPlugin))]
        public List<AnotherSubConfig> SubConfigs { get; set; }    
    }

    [DataContract]
    public class AnotherSubConfig : IPluginConfig
    {
        /// <summary>
        /// Name of the component represented by this entry
        /// </summary>
        [DataMember]
        public string PluginName { get; set; }
    }

    [DataContract]
    public class AnotherSubConfig2 : AnotherSubConfig
    {
        [DataMember]
        public int Value { get; set; }
    }
}