using Marvin.AbstractionLayer.UI;
using Marvin.Container;

namespace Marvin.Products.UI.Interaction
{
    [PluginFactory(typeof(ProductDetailsComponentSelector))]
    internal interface IProductDetailsFactory : IDetailsFactory<IProductDetails>
    {

    }
}