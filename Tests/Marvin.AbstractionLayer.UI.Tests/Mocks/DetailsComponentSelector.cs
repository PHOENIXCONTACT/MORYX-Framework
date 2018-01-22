using Marvin.Container;

namespace Marvin.AbstractionLayer.UI.Tests
{
    public class DetailsComponentSelector : DetailsComponentSelector<IDetailsComponent, IInteractionController>
    {
        public DetailsComponentSelector(IContainer container, IInteractionController controller) : base(container, controller)
        {
        }
    }

    public class InteractionControllerMock : IInteractionController
    {
        
    }
}
