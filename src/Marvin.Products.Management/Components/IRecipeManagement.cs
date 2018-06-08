using System;
using System.Collections.Generic;
using Marvin.AbstractionLayer;
using Marvin.Workflows;

namespace Marvin.Products.Management
{
    /// <summary>
    /// Component to handle all recipe operations
    /// </summary>
    internal interface IRecipeManagement
    {
        /// <summary>
        /// Loads the recipe by the given identifier
        /// </summary>
        IProductRecipe Get(long recipeId);

        /// <summary>
        /// Will load all recipes by the given product
        /// </summary>
        IReadOnlyList<IProductRecipe> GetAllByProduct(IProduct product);

        /// <summary>
        /// Retrieves a recipe for the product
        /// </summary>
        IReadOnlyList<IProductRecipe> GetRecipes(IProduct product, RecipeClassification classifications);

        /// <summary>
        /// Retrieves a full recipe for a combination of product and workplan.
        /// </summary>
        /// <param name="productId">The product's ID</param>
        /// <param name="workplanId">The worklan's ID</param>
        /// <returns>The recipe or null if there is no recipe for the given combination of product and workplan.</returns>
        IProductRecipe GetRecipe(long productId, long workplanId);

        /// <summary>
        /// A recipe was changed, give users the chance to update their reference
        /// </summary>
        event EventHandler<IRecipe> RecipeChanged;

        /// <summary>
        /// Create a full recipe for a combination of product and workplan.
        /// </summary>
        IProductRecipe Create(long productId, long workplanId, string name);

        /// <summary>
        /// Save recipe to DB
        /// </summary>
        long Save(IProductRecipe instance);

        /// <summary>
        /// Saves multiple recipies
        /// </summary>
        void Save(long productId, ICollection<IProductRecipe> recipes);
    }
}