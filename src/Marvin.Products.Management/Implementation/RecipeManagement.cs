using System;
using System.Collections.Generic;
using Marvin.AbstractionLayer;
using Marvin.Container;
using Marvin.Model;

namespace Marvin.Products.Management
{
    [Component(LifeCycle.Singleton, typeof(IRecipeManagement))]
    internal class RecipeManagement : IRecipeManagement
    {
        #region Dependencies

        public IProductStorage Storage { get; set; }

        public IUnitOfWorkFactory UowFactory { get; set; }

        #endregion

        public IProductRecipe Get(long recipeId)
        {
            return Storage.LoadRecipe(recipeId);
        }

        public IReadOnlyList<IProductRecipe> GetAllByProduct(IProduct product)
        {
            return Storage.LoadRecipes(product.Id, RecipeClassification.CloneFilter);
        }

        public IReadOnlyList<IProductRecipe> GetRecipes(IProduct product, RecipeClassification classification)
        {
            return Storage.LoadRecipes(product.Id, classification);
        }

        public long Save(IProductRecipe recipe)
        {
            var saved =  Storage.SaveRecipe(recipe);
            RaiseRecipeChanged(recipe);
            return saved;
        }

        public void Save(long productId, ICollection<IProductRecipe> recipes)
        {
            Storage.SaveRecipes(productId, recipes);
            foreach (var recipe in recipes)
                RaiseRecipeChanged(recipe);
        }

        private void RaiseRecipeChanged(IRecipe recipe)
        {
            // This must never be null
            // ReSharper disable once PossibleNullReferenceException
            RecipeChanged(this, recipe);
        }
        public event EventHandler<IRecipe> RecipeChanged;
    }
}