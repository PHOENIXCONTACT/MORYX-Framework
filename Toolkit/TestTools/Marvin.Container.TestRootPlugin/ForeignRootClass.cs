using Marvin.Container.TestTools;

namespace Marvin.Container.TestRootPlugin
{
    [DependencyRegistration(InstallerMode.All)]
    [Plugin(LifeCycle.Singleton, typeof(IRootClass), Name = PluginName)]
    public class ForeignRootClass : IRootClass
    {
        public const string PluginName = "ForeignRootClass";

        public string GetName()
        {
            return PluginName;
        }

        public void Initialize(RootClassFactoryConfig config)
        {
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public void Start()
        {
            throw new System.NotImplementedException();
        }

        public IConfiguredComponent ConfiguredComponent { get; set; }
    }
}