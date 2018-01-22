using Marvin.Resources.UI.Interaction.ResourceInteraction;

namespace Marvin.Resources.UI.Interaction
{
    /// <summary>
    /// Resource view model for the <see cref="Resource"/>
    /// </summary>
    public class ResourceViewModel : ResourceViewModelBase
    {
        internal ResourceModel Model { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceViewModel"/> class.
        /// </summary>
        internal ResourceViewModel(ResourceModel model) : base(model)
        {
            Model = model;
        }
    }
}