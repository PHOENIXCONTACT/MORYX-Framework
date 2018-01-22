using System;
using System.Collections.Generic;
using System.Linq;
using Marvin.AbstractionLayer;
using Marvin.Container;
using Marvin.Model;
using Marvin.Products.Model;
using Marvin.Workflows;

namespace Marvin.Products.Management
{
    [Component(LifeCycle.Singleton, typeof(IRecipeManagement))]
    internal class RecipeManagement : IRecipeManagement
    {
        #region Dependencies

        public IProductStorage Storage { get; set; }

        public IUnitOfWorkFactory UowFactory { get; set; }

        #endregion

        // Forward to serialization on injection
        public ICustomization Customization
        {
            get { return Serialization.Customization; }
            set { Serialization.Customization = value; }
        }

        private static readonly PartialSerialization<IProductRecipe> Serialization = new PartialSerialization<IProductRecipe>();

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

        public IProductRecipe GetRecipe(long productId, long workplanId)
        {
            using (var uow = UowFactory.Create())
            {
                var recipeId = (from entity in uow.GetRepository<IProductRecipeEntityRepository>().Linq
                                where entity.ProductId == productId && entity.WorkplanId == workplanId
                                select entity.Id).FirstOrDefault();

                return recipeId == 0 ? null : Storage.LoadRecipe(recipeId);
            }
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

        public IProductRecipe Create(long productId, long workplanId, string name)
        {
            var recipe = Customization.RecipePrototype(nameof(ProductRecipe));
            recipe.Name = name;
            recipe.Product = new ProductReference(productId);
            recipe.Workplan = new Workplan { Id = workplanId };

            Storage.SaveRecipe(recipe);

            return recipe;
        }

        private void RaiseRecipeChanged(IRecipe recipe)
        {
            // This must never by null
            // ReSharper disable once PossibleNullReferenceException
            RecipeChanged(this, recipe);
        }
        public event EventHandler<IRecipe> RecipeChanged;
    }
}