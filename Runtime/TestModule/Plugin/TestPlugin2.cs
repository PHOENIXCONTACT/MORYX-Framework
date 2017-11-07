using Marvin.Container;
using Marvin.Modules.ModulePlugins;

namespace Marvin.TestModule
{
    [ExpectedConfig(typeof(TestPluginConfig2))]
    [Plugin(LifeCycle.Singleton, typeof(ITestPlugin), Name = ComponentName)]
    public class TestPlugin2 : ITestPlugin
    {
        public const string ComponentName = "TestPlugin2";


        public void Initialize(TestPluginConfig config)
        {
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}