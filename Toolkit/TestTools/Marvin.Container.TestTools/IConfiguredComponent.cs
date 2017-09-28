using Marvin.Modules.ModulePlugins;

namespace Marvin.Container.TestTools
{
    public interface IConfiguredComponent : IConfiguredModulePlugin<ComponentConfig>
    {
        string GetName();
    }
}