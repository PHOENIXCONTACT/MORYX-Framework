using System.Runtime.Serialization;
using Marvin.Modules;

namespace Marvin.TestModule
{
    [DataContract]
    public class TestSubPluginConfig : IPluginConfig
    {
        public virtual string PluginName { get { return TestSubPlugin.ComponentName; } }
    }
}