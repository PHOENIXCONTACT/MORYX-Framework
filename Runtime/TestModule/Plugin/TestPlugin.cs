using Marvin.Container;
using Marvin.Modules.ModulePlugins;

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

        public void Dispose()
        {
        }

        public void Start()
        {
        }
    }
}