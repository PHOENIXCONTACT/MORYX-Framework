using Marvin.Modules.ModulePlugins;

namespace Marvin.Container.TestTools
{
    public interface IConfiguredComponent : IConfiguredPlugin<ComponentConfig>
    {
        string GetName();
    }
}