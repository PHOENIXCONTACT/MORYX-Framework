using Marvin.Container.TestTools;

namespace Marvin.Container.TestRootPlugin
{
    [DependencyRegistration(InstallerMode.All)]
    [Plugin(LifeCycle.Singleton, typeof(IRootClass), Name = PluginName)]
    public class ForeignRootClass : IRootClass
    {
        public const string PluginName = "ForeignRootClass";

        public IConfiguredComponent ConfiguredComponent { get; set; }

        public string GetName()
        {
            return PluginName;
        }

        public void Initialize(RootClassFactoryConfig config)
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