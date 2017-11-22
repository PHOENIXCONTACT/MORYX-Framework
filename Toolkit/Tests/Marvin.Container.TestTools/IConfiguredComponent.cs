using Marvin.Modules;

namespace Marvin.Container.TestTools
{
    public interface IConfiguredComponent : IConfiguredPlugin<ComponentConfig>
    {
        string GetName();
    }
}