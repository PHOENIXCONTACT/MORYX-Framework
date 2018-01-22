using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Marvin.Resources.UI.Interaction.ResourceInteraction;

namespace Marvin.Resources.UI.Interaction
{
    /// <summary>
    /// View model for the resource type
    /// </summary>
    public class ResourceTypeViewModel : PropertyChangedBase, IResourceTypeViewModel
    {
        private readonly ResourceTypeModel _model;
        private ResourceTypeViewModel[] _derivedTypes;

        internal ResourceTypeViewModel(ResourceTypeModel model)
        {
            _model = model;
            Constructors = model.Constructors.Select(cstr => new ConstructorViewModel(cstr)).ToArray();
            if (Constructors.Length > 0)
                Constructors[0].IsSelected = true;
            _derivedTypes = TransfromChildren(model.DerivedTypes);
        }

        /// <summary>
        /// The name of the resource 
        /// </summary>
        public string Name => _model.Name;

        /// <summary>
        /// Display name of the type
        /// </summary>
        public string DisplayName => _model.DisplayName ?? _model.Name;

        /// <summary>
        /// Description of the type
        /// </summary>
        public string Description => _model.Description;

        /// <summary>
        /// Flag if this type can be instantiated
        /// </summary>
        public bool Creatable => _model.Creatable;

        /// <summary>
        /// Constructors of this type
        /// </summary>
        public ConstructorViewModel[] Constructors { get; private set; }

        /// <summary>
        /// The children of the current tree item
        /// </summary>
        public ResourceTypeViewModel[] DerivedTypes
        {
            get { return _derivedTypes; }
            private set
            {
                _derivedTypes = value;
                NotifyOfPropertyChange();
            }
        }

        IEnumerable<IResourceTypeViewModel> IResourceTypeViewModel.DerivedTypes => DerivedTypes;

        /// <summary>
        /// Transfroms the children tree items.
        /// </summary>
        private static ResourceTypeViewModel[] TransfromChildren(IList<ResourceTypeModel> children)
        {
            var result = new ResourceTypeViewModel[children.Count];

            for (int i = 0; i < children.Count; i++)
            {
                var current = children[i];
                result[i] = new ResourceTypeViewModel(current)
                {
                    DerivedTypes = TransfromChildren(current.DerivedTypes)
                };
            }

            return result;
        }
    }
}