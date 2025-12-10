// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq.Expressions;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Modules;

namespace Moryx.Products.Management
{
    /// <summary>
    /// API for the application specific product storage
    /// </summary>
    public interface IProductStorage : IPlugin
    {
        /// <summary>
        /// Get products by query
        /// </summary>
        Task<IReadOnlyList<ProductType>> LoadTypesAsync(ProductQuery query);

        /// <summary>
        /// Load product instance by id
        /// </summary>
        Task<ProductType> LoadTypeAsync(long id);

        /// <summary>
        /// Load product by identity. This method supports loading a products latest revision
        /// </summary>
        Task<ProductType> LoadTypeAsync(IIdentity identity);

        /// <summary>
        /// Save a type to the storage
        /// </summary>
        Task<long> SaveTypeAsync(ProductType modifiedInstance);

        /// <summary>
        /// Get instances by id
        /// </summary>
        /// <returns>The instance with the id when it exists.</returns>
        Task<IReadOnlyList<ProductInstance>> LoadInstancesAsync(params long[] id);

        /// <summary>
        /// Load instances using filter expression
        /// </summary>
        Task<IReadOnlyList<TInstance>> LoadInstancesAsync<TInstance>(Expression<Func<TInstance, bool>> selector);

        /// <summary>
        /// Load instances using ProductType
        /// </summary>
        Task<IReadOnlyList<ProductInstance>> LoadInstancesAsync(ProductType productType);

        /// <summary>
        /// Updates the database from the instance
        /// </summary>
        Task SaveInstancesAsync(ProductInstance[] productInstance);

        /// <summary>
        /// Loads a recipe from the storage
        /// </summary>
        Task<IProductRecipe> LoadRecipeAsync(long recipeId);

        /// <summary>
        /// Loads all recipes from the storage.
        /// </summary>
        Task<IReadOnlyList<IProductRecipe>> LoadRecipesAsync(long productId, RecipeClassification classification);

        /// <summary>
        /// Saves the recipe of the product
        /// </summary>
        Task<long> SaveRecipeAsync(IProductRecipe recipe);

        /// <summary>
        /// Save multiple recipes at once
        /// </summary>
        Task SaveRecipesAsync(IReadOnlyList<IProductRecipe> recipes);

        /// <summary>
        /// Load types using filter expression
        /// </summary>
        Task<IReadOnlyList<TType>> LoadTypesAsync<TType>(Expression<Func<TType, bool>> selector);

        /// <summary>
        /// Remove recipe by given recipeId
        /// </summary>
        Task RemoveRecipeAsync(long recipeId);

        /// <summary>
        ///
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        ProductTypeWrapper GetTypeWrapper(string typeName);

        /// <summary>
        /// Create instance of a recipe
        /// </summary>
        /// <param name="recipeType">Full name of the recipe type</param>
        /// <returns></returns>
        IProductRecipe CreateRecipe(string recipeType);

        /// <summary>
        /// Checks database's connection by making an initial attempt
        /// </summary>
        /// <returns></returns>
        Task CheckDatabase();
    }

}
