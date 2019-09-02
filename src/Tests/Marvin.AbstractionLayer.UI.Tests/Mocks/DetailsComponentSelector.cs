using Marvin.Container;

namespace Marvin.AbstractionLayer.UI.Tests
{
    public class DetailsComponentSelector : DetailsComponentSelector<IDetailsComponent>
    {
        public DetailsComponentSelector(IContainer container) : base(container)
        {
        }
    }
}
