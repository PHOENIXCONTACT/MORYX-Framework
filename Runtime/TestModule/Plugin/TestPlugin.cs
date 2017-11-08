using Marvin.Container;
using Marvin.Modules;

namespace Marvin.TestModule
{
    [ExpectedConfig(typeof(TestPluginConfig))]
    [Plugin(LifeCycle.Singleton, typeof(ITestPlugin), Name = ComponentName)]
    public class TestPlugin : ITestPlugin
    {
        public const string ComponentName = "TestPlugin";

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