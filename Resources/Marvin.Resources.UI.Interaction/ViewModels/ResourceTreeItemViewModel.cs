using System.Collections.Generic;
using System.Linq;
using Marvin.AbstractionLayer.UI;
using Marvin.Resources.UI.Interaction.ResourceInteraction;

namespace Marvin.Resources.UI.Interaction
{
    /// <summary>
    /// Tree item view model used for the resources tree
    /// </summary>
    public class ResourceTreeItemViewModel : ResourceViewModelBase, ITreeItemViewModel
    {
        private ResourceTreeItemViewModel[] _children;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceTreeItemViewModel"/> class.
        /// </summary>
        internal ResourceTreeItemViewModel(ResourceModel model) : base(model)
        {
            var childrenReference = model.References.First(r => r.RelationType == ResourceRelationType.ParentChild);
            
            PossibleChildren = childrenReference.SupportedTypes.Select(type => new ResourceTypeViewModel(type)).ToArray();

            _children = childrenReference.Targets.Select(child => new ResourceTreeItemViewModel(child)).ToArray();
        }

        /// <summary>
        /// The children of the current tree item
        /// </summary>
        public ResourceTreeItemViewModel[] Children
        {
            get { return _children; }
            private set
            {
                _children = value; 
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Possible types used as children
        /// </summary>
        public ResourceTypeViewModel[] PossibleChildren { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this item is expanded.
        /// </summary>
        public bool IsExpanded { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this item is selected.
        /// </summary>
        public bool IsSelected { get; set; }
    }
}