using Marvin.Modules;

namespace Marvin.Container.TestTools
{
    public interface IRootClass : IConfiguredPlugin<RootClassFactoryConfig>
    {
        IConfiguredComponent ConfiguredComponent { get; set; }
        string GetName();
    }
}