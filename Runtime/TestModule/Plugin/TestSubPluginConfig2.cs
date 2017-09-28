using System.Runtime.Serialization;

namespace Marvin.TestModule
{
    [DataContract]
    public class TestSubPluginConfig2 : TestSubPluginConfig
    {
        public override string PluginName { get { return TestSubPlugin2.ComponentName; } }
         
    }
}