using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.Runtime.Configuration;

namespace Marvin.TestModule
{
    public class TestPluginConfig2 : TestPluginConfig
    {
        /// <summary>
        /// The name of the component.
        /// </summary>
        [DataMember]
        [PluginNameSelector(typeof(ITestPlugin))]
        [DefaultValue(TestPlugin2.ComponentName)]
        public override string PluginName { get; set; }
    }
}