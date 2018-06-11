using Marvin.Container;

namespace Marvin.AbstractionLayer.UI.Tests
{
    [PluginFactory(typeof(DetailsComponentSelector))]
    public interface IDetailsComponentFactory : IDetailsFactory<IDetailsComponent>
    {
        
    }
}