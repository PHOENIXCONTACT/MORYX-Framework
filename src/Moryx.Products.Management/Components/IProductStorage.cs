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
    public interface IProductStorage : IAsyncPlugin
    {
        /// <summary>
        /// Get products by query
        /// </summary>
        Task<IReadOnlyList<ProductType>> LoadTypesAsync(ProductQuery query, CancellationToken cancellationToken = default);

        /// <summary>
        /// Load product instance by id
        /// </summary>
        Task<ProductType> LoadTypeAsync(long id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Load product by identity. This method supports loading a products latest revision
        /// </summary>
        Task<ProductType> LoadTypeAsync(IIdentity identity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Save a type to the storage
        /// </summary>
        Task<long> SaveTypeAsync(ProductType modifiedInstance, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get instances by id
        /// </summary>
        /// <returns>The instance with the id when it exists.</returns>
        Task<IReadOnlyList<ProductInstance>> LoadInstancesAsync(long[] ids, CancellationToken cancellationToken = default);

        /// <summary>
        /// Load instances using filter expression
        /// </summary>
        Task<IReadOnlyList<TInstance>> LoadInstancesAsync<TInstance>(Expression<Func<TInstance, bool>> selector, CancellationToken cancellationToken = default);

        /// <summary>
        /// Load instances using ProductType
        /// </summary>
        Task<IReadOnlyList<ProductInstance>> LoadInstancesAsync(ProductType productType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the database from the instance
        /// </summary>
        Task SaveInstancesAsync(ProductInstance[] productInstance, CancellationToken cancellationToken = default);

        /// <summary>
        /// Loads a recipe from the storage
        /// </summary>
        Task<IProductRecipe> LoadRecipeAsync(long recipeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Loads all recipes from the storage.
        /// </summary>
        Task<IReadOnlyList<IProductRecipe>> LoadRecipesAsync(long productId, RecipeClassification classification, CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves the recipe of the product
        /// </summary>
        Task<long> SaveRecipeAsync(IProductRecipe recipe, CancellationToken cancellationToken = default);

        /// <summary>
        /// Save multiple recipes at once
        /// </summary>
        Task SaveRecipesAsync(IReadOnlyList<IProductRecipe> recipes, CancellationToken cancellationToken = default);

        /// <summary>
        /// Load types using filter expression
        /// </summary>
        Task<IReadOnlyList<TType>> LoadTypesAsync<TType>(Expression<Func<TType, bool>> selector, CancellationToken cancellationToken = default);

        /// <summary>
        /// Remove recipe by given recipeId
        /// </summary>
        Task RemoveRecipeAsync(long recipeId, CancellationToken cancellationToken = default);

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
        Task CheckDatabase(CancellationToken cancellationToken = default);
    }

}
