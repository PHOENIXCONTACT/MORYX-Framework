// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;

namespace Moryx.Products.Management
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
        /// Retrieves a recipe for the product
        /// </summary>
        IReadOnlyList<IProductRecipe> GetRecipes(ProductType productType, RecipeClassification classifications);

        /// <summary>
        /// A recipe was changed, give users the chance to update their reference
        /// </summary>
        event EventHandler<IRecipe> RecipeChanged;

        /// <summary>
        /// Save recipe to DB
        /// </summary>
        long Save(IProductRecipe instance);

        /// <summary>
        /// Remove the recipe by the given identifier
        /// </summary>
        void Remove(long recipeId);

        /// <summary>
        /// Create instance of a recipe
        /// </summary>
        /// <param name="recipeType">Full name of the recipe type</param>
        /// <returns></returns>
        IProductRecipe CreateRecipe(string recipeType);
    }
}
