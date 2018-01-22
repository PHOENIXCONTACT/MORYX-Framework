using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;
using Marvin.Controls;
using Marvin.Products.UI.Interaction.InteractionSvc;

namespace Marvin.Products.UI.Interaction
{
    /// <summary>
    /// View model representing the product
    /// </summary>
    public class ProductViewModel : PropertyChangedBase, IEditableObject
    {
        internal ProductModel Model { get; }
        private string _name;
        
        internal ProductViewModel(ProductModel model)
        {
            Model = model;
            CopyFromModel();
        }

        private void CopyFromModel()
        {
            Name = Model.Name;
        }

        private void CopyToModel()
        {
            Model.Name = Name;
        }

        /// <summary>
        /// Gets the unique identifier of the product.
        /// </summary>
        public long Id => Model.Id;

        /// <summary>
        /// The identifier of the product 
        /// </summary>
        public string Identifier => Model.Identifier;

        /// <summary>
        /// Revision of th product
        /// </summary>
        public short Revision => Model.Revision;

        /// <summary>
        /// Current state of the product
        /// </summary>
        public string State => Model.State.ToString();

        /// <summary>
        /// Gets the full identifier of the product IIIII-RR
        /// </summary>
        public string FullIdentifier => $"{Model.Identifier}-{Model.Revision:D2}";

        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value; 
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets the type of the entry.
        /// </summary>
        public string Type => Model.Type;

        ///
        public void BeginEdit()
        {

        }

        ///
        public void EndEdit()
        {
            CopyToModel();
        }

        ///
        public void CancelEdit()
        {
            CopyFromModel();
        }
    }
}