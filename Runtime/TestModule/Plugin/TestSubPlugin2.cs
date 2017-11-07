using Marvin.Container;
using Marvin.Modules.ModulePlugins;

namespace Marvin.TestModule
{
    [ExpectedConfig(typeof(TestSubPluginConfig2))]
    [Plugin(LifeCycle.Singleton, typeof(ITestSubPlugin), Name = ComponentName)]
    public class TestSubPlugin2 : ITestSubPlugin
    {
        public const string ComponentName = "TestSubPlugin2";

        public void Initialize(TestSubPluginConfig config)
        {
        }

        public void Dispose()
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