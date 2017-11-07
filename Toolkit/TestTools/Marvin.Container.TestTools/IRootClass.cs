using Marvin.Modules.ModulePlugins;

namespace Marvin.Container.TestTools
{
    public interface IRootClass : IConfiguredPlugin<RootClassFactoryConfig>
    {
        IConfiguredComponent ConfiguredComponent { get; set; }
        string GetName();
    }
}