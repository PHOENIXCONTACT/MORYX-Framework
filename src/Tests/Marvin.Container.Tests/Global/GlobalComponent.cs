using Marvin.Model;

namespace Marvin.Container.Tests
{
    [GlobalComponent(LifeCycle.Singleton)]
    internal class GlobalComponent
    {
    }

    [Model("Some Namespace")]
    internal class ModelComponent
    {
        
    }
}