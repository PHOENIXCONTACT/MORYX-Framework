using System.ComponentModel;
using Caliburn.Micro;
using Marvin.Products.UI.Interaction.InteractionSvc;
using Marvin.Serialization;

namespace Marvin.Products.UI.Interaction
{
    /// <summary>
    /// View model for recipes of a product
    /// </summary>
    public class RecipeViewModel : PropertyChangedBase, IEditableObject
    {
        private string _name;

        internal RecipeModel Model { get; set; }

        /// <summary>
        /// Ingrediants of this recipe model
        /// </summary>
        protected Entry[] IngredientsModel
        {
            get { return Model.Ingredients; }
            set { Model.Ingredients = value; }
        }

        internal void Initialize(RecipeModel model)
        {
            Model = model;
            CopyFromModel();
        }

        /// <summary>
        /// Copy all model information to the defined view model properties
        /// </summary>
        protected virtual void CopyFromModel()
        {
            Name = Model.Name;
        }

        /// <summary>
        /// Copy all local information to the model to save it on the server
        /// </summary>
        protected virtual void CopyToModel()
        {
            Model.Name = Name;
        }

        /// <summary>
        /// Identifier of the recipe
        /// </summary>
        public long Id => Model.Id;

        /// <summary>
        /// Name of the recipe
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

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