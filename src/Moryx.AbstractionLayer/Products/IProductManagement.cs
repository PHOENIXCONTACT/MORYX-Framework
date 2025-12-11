// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq.Expressions;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Workplans;

namespace Moryx.AbstractionLayer.Products
{
    /// <summary>
    /// Merged facade for products and product instances
    /// </summary>
    public interface IProductManagement : IRecipeProvider, IWorkplans
    {
        /// <summary>
        /// Get product types based on a query
        /// </summary>
        Task<IReadOnlyList<ProductType>> LoadTypesAsync(ProductQuery query);

        /// <summary>
        /// Load product type by id
        /// </summary>
        /// <exception cref="ProductNotFoundException">Thrown when the product with the given id doesn't exist.</exception>
        Task<ProductType> LoadTypeAsync(long id);

        /// <summary>
        /// Load product type by identity
        /// </summary>
        /// <exception cref="ProductNotFoundException">Thrown when the product with the given id doesn't exist.</exception>
        Task<ProductType> LoadTypeAsync(IIdentity identity);

        /// <summary>
        /// Event raised when a product type changed
        /// </summary>
        event EventHandler<ProductType> TypeChanged;

        /// <summary>
        /// Duplicate a product under a new identity
        /// </summary>
        /// <exception cref="IdentityConflictException">Thrown when the new identity causes conflicts</exception>
        Task<ProductType> DuplicateAsync(ProductType template, IIdentity newIdentity);

        /// <summary>
        /// Save a product type
        /// </summary>
        Task<long> SaveTypeAsync(ProductType modifiedInstance);

        /// <summary>
        /// All importers and their parameters currently configured in the machine
        /// </summary>
        IDictionary<string, object> Importers { get; }

        /// <summary>
        /// Import product types for the given parameters with the named importer
        /// </summary>
        Task<ProductImportResult> ImportAsync(string importerName, object parameters);

        /// <summary>
        /// Retrieves the current recipe for this product
        /// </summary>
        Task<IReadOnlyList<IProductRecipe>> GetRecipesAsync(ProductType productType, RecipeClassification classification);

        /// <summary>
        /// Saves given recipe to the storage
        /// </summary>
        Task<long> SaveRecipeAsync(IProductRecipe recipe);

        /// <summary>
        /// Saves given recipe to the storage
        /// </summary>
        Task SaveRecipesAsync(IReadOnlyList<IProductRecipe> recipes);

        /// <summary>
        /// Create an product instance of given product
        /// </summary>
        /// <param name="productType">Product to instantiate</param>
        /// <returns>Unsaved instance</returns>
        Task<ProductInstance> CreateInstanceAsync(ProductType productType);

        /// <summary>
        /// Create an product instance of given product
        /// </summary>
        /// <param name="productType">Product type to instantiate</param>
        /// <param name="save">Flag if new instance should already be saved</param>
        /// <returns>New instance</returns>
        Task<ProductInstance> CreateInstanceAsync(ProductType productType, bool save);

        /// <summary>
        /// Get an product instance with the given id.
        /// </summary>
        /// <param name="id">The id for the product instance which should be searched for.</param>
        /// <returns>The product instance with the id when it exists.</returns>
        Task<ProductInstance> GetInstanceAsync(long id);

        /// <summary>
        /// Get an instance with this identity
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="identity"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown when there is more than one product with the given <paramref name="identity"/></exception>
        Task<ProductInstance> GetInstanceAsync(IIdentity identity);

        /// <summary>
        /// Get only instances that match a certain condition, similar to SingleOrDefault
        /// </summary>
        Task<TInstance> GetInstanceAsync<TInstance>(Expression<Func<TInstance, bool>> selector)
            where TInstance : ProductInstance;

        /// <summary>
        /// Updates the database from the product instance
        /// </summary>
        Task SaveInstanceAsync(ProductInstance productInstance);

        /// <summary>
        /// Updates the database from the product instance
        /// </summary>
        Task SaveInstancesAsync(ProductInstance[] productInstances);

        /// <summary>
        /// Get instances with the given ids.
        /// </summary>
        /// <param name="ids">The IDs of instances that should be loaded</param>
        /// <returns>The instance with the id when it exists.</returns>
        Task<IReadOnlyList<ProductInstance>> GetInstancesAsync(long[] ids);

        /// <summary>
        /// Get all instances that match a certain
        /// </summary>
        Task<IReadOnlyList<TInstance>> GetInstancesAsync<TInstance>(Expression<Func<TInstance, bool>> selector)
            where TInstance : ProductInstance;

        /// <summary>
        /// Load types using filter expression
        /// </summary>
        Task<IReadOnlyList<TType>> LoadTypesAsync<TType>(Expression<Func<TType, bool>> selector);

        /// <summary>
        /// List of available product types
        /// </summary>
        IReadOnlyList<Type> ProductTypes { get; }

        /// <summary>
        /// List of available recipes
        /// </summary>
        IReadOnlyList<Type> RecipeTypes { get; }

        /// <summary>
        /// Delete a product by its id
        /// </summary>
        Task<bool> DeleteProductAsync(long id);

        /// <summary>
        /// Remove Recipe by given id
        /// </summary>
        Task RemoveRecipeAsync(long recipeId);

        /// <summary>
        /// Create instance of a recipe
        /// </summary>
        /// <param name="recipeType">Full name of the recipe type</param>
        /// <returns></returns>
        IProductRecipe CreateRecipe(string recipeType);

        /// <summary>
        /// Return type wrapper to a type
        /// </summary>
        /// <param name="typeName">Full name of the type</param>
        /// <returns></returns>
        ProductTypeWrapper GetTypeWrapper(string typeName);
    }

}
