using System.Collections.Generic;
using Caliburn.Micro;
using System.Linq;
using System.Text.RegularExpressions;
using Marvin.AbstractionLayer.UI;
using Marvin.Products.UI.Interaction.InteractionSvc;

namespace Marvin.Products.UI.Interaction
{
    /// <summary>
    /// View model for the product structure entries
    /// Will be used within the product tree
    /// </summary>
    public class StructureEntryViewModel :PropertyChangedBase,  ITreeItemViewModel
    {
        private readonly ProductStructureEntry _model;
        private StructureEntryViewModel[] _branches;

        /// <summary>
        /// Create ViewModel using all its branches
        /// </summary>
        internal StructureEntryViewModel(ProductStructureEntry model) : this(model, true)
        {
        }

        /// <summary>
        /// Create ViewModel and specify wether to include the branches
        /// </summary>
        private StructureEntryViewModel(ProductStructureEntry model, bool includeBranches)
        {
            _model = model;
            if (includeBranches)
                Branches = TransfromBranches(model.Branches);
        }

        /// <summary>
        /// Create a clone of the view model
        /// </summary>
        public StructureEntryViewModel Clone()
        {
            return new StructureEntryViewModel(_model, false)
            {
                Branches = Branches
            };
        }

        /// <summary>
        /// Gets the MaterialNumber of the product
        /// </summary>
        public string MaterialNumber
        {
            get { return _model.MaterialNumber; }
        }

        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        public long Id => _model.Id;

        /// <summary>
        /// Gets the identifier of the product
        /// </summary>
        public string Identifier => $"{_model.MaterialNumber}-{_model.Revision:D2}";

        /// <summary>
        /// Gets the name of the product including identity and revision.
        /// </summary>
        public string Name
        {
            get
            {
                return _model.BranchType == BranchType.Product ? $"{_model.MaterialNumber}-{_model.Revision:D2} {_model.Name}" : _model.Name;
               
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is a product
        /// </summary>
        public bool IsProduct => _model.BranchType == BranchType.Product;

        /// <summary>
        /// Gets the type of the entry.
        /// </summary>
        public string Type => _model.Type;

        /// <summary>
        /// Transfroms the children tree items.
        /// </summary>
        private static StructureEntryViewModel[] TransfromBranches(IList<ProductStructureEntry> children)
        {
            var result = new StructureEntryViewModel[children.Count];

            for (var i = 0; i < children.Count; i++)
            {
                var current = children[i];
                result[i] = new StructureEntryViewModel(current)
                {
                    Branches = TransfromBranches(current.Branches)
                };
            }

            return result;
        }

        /// <summary>
        /// The children of the current tree item
        /// </summary>
        public StructureEntryViewModel[] Branches
        {
            get { return _branches; }
            set
            {
                _branches = value;
                NotifyOfPropertyChange();
            }
        }

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
