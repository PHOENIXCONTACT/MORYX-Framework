using Marvin.Container;
using Marvin.Modules.ModulePlugins;

namespace Marvin.TestModule
{
    [ExpectedConfig(typeof(TestPluginConfig1))]
    [Plugin(LifeCycle.Singleton, typeof(ITestPlugin), Name = ComponentName)]
    public class TestPlugin1 : ITestPlugin
    {
        public const string ComponentName = "TestPlugin1";

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