using System.Collections.Generic;
using System.Threading.Tasks;
using Marvin.AbstractionLayer.UI;
using Marvin.Products.UI.Interaction.InteractionSvc;
using Marvin.Serialization;

namespace Marvin.Products.UI.Interaction
{
    /// <summary>
    /// Base class for the product details view model
    /// </summary>
    public abstract class ProductDetailsViewModelBase : EditModeViewModelBase<ProductViewModel>, IProductDetails
    {
        internal IProductsController ProductsController { get; private set; }

        /// <summary>
        /// Model visualized by this view model
        /// </summary>
        private ProductModel _model;

        #region Properties

        /// <summary>
        /// Represents the product properties
        /// </summary>
        public Entry[] ProductProperties
        {
            get { return _model.Properties; }
            protected set { _model.Properties = value; }
        }

        /// <summary>
        /// List of recipes of the product
        /// </summary>
        public RecipeViewModel[] Recipes { get; private set; }

        /// <summary>
        /// The default used recipe of this product
        /// </summary>
        public RecipeViewModel DefaultRecipe { get; set; }

        ///
        public long ProductId => EditableObject?.Id ?? 0;

        #endregion


        /// <inheritdoc />
        public void Initialize(IInteractionController controller, string typeName)
        {
            base.Initialize();
            
            ProductsController = (IProductsController)controller;
        }

        ///
        public async Task Load(long productId)
        {
            _model = await ProductsController.GetDetails(productId);

            EditableObject = new ProductViewModel(_model);

            // Convert recipes
            var recipeViewModels = new List<RecipeViewModel>();
            foreach (var recipe in _model.Recipes)
            {
                var vm = CreateRecipeViewModelPrototype();
                vm.Initialize(recipe);

                recipeViewModels.Add(vm);

                // In additional put the current recipe in a separate property
                if (recipe.IsDefault)
                    DefaultRecipe = vm;
            }
            Recipes = recipeViewModels.ToArray();
        }

        /// <summary>
        /// Create a prototype of a new recipe view model which can be changed for special recipe view models
        /// </summary>
        /// <returns>Returns a new view model which can be a specialized recipe view model type</returns>
        protected virtual RecipeViewModel CreateRecipeViewModelPrototype()
        {
            return new RecipeViewModel();
        }

        ///
        protected override async Task OnSave(object parameters)
        {
            var product = await ProductsController.Save(_model);
            EditableObject = new ProductViewModel(product);
            NotifyOfPropertyChange(() => EditableObject);

            await base.OnSave(parameters);
        }
    }
}
