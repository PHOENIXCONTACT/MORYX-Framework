// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq.Expressions;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Workplans;

namespace Moryx.AbstractionLayer.Products;

/// <summary>
/// Merged facade for products and product instances
/// </summary>
public interface IProductManagement : IRecipeProvider, IWorkplans
{
    /// <summary>
    /// Get product types based on a query
    /// </summary>
    Task<IReadOnlyList<ProductType>> LoadTypesAsync(ProductQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Load product type by id
    /// </summary>
    /// <exception cref="ProductNotFoundException">Thrown when the product with the given id doesn't exist.</exception>
    Task<ProductType> LoadTypeAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Load product type by identity
    /// </summary>
    /// <exception cref="ProductNotFoundException">Thrown when the product with the given id doesn't exist.</exception>
    Task<ProductType> LoadTypeAsync(IIdentity identity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Event raised when a product type changed
    /// </summary>
    event EventHandler<ProductType> TypeChanged;

    /// <summary>
    /// Duplicate a product under a new identity
    /// </summary>
    /// <exception cref="IdentityConflictException">Thrown when the new identity causes conflicts</exception>
    Task<ProductType> DuplicateTypeAsync(ProductType template, IIdentity newIdentity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Save a product type
    /// </summary>
    Task<long> SaveTypeAsync(ProductType modifiedInstance, CancellationToken cancellationToken = default);

    /// <summary>
    /// All importers and their parameters currently configured in the machine
    /// </summary>
    IDictionary<string, object> Importers { get; }

    /// <summary>
    /// Import product types for the given parameters with the named importer
    /// </summary>
    Task<ProductImportResult> ImportAsync(string importerName, object parameters, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the current recipe for this product
    /// </summary>
    Task<IReadOnlyList<IProductRecipe>> LoadRecipesAsync(ProductType productType, RecipeClassification classification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves given recipe to the storage
    /// </summary>
    Task<long> SaveRecipeAsync(IProductRecipe recipe, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves given recipe to the storage
    /// </summary>
    Task SaveRecipesAsync(IReadOnlyList<IProductRecipe> recipes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create an product instance of given product
    /// </summary>
    /// <param name="productType">Product to instantiate</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <returns>Unsaved instance</returns>
    Task<ProductInstance> CreateInstanceAsync(ProductType productType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create an product instance of given product
    /// </summary>
    /// <param name="productType">Product type to instantiate</param>
    /// <param name="save">Flag if new instance should already be saved</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <returns>New instance</returns>
    Task<ProductInstance> CreateInstanceAsync(ProductType productType, bool save, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get an product instance with the given id.
    /// </summary>
    /// <param name="id">The id for the product instance which should be searched for.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <returns>The product instance with the id when it exists.</returns>
    Task<ProductInstance> LoadInstanceAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get an instance with this identity
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="identity"/> is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when there is more than one product with the given <paramref name="identity"/></exception>
    Task<ProductInstance> LoadInstanceAsync(IIdentity identity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get only instances that match a certain condition, similar to SingleOrDefault
    /// </summary>
    Task<TInstance> LoadInstanceAsync<TInstance>(Expression<Func<TInstance, bool>> selector, CancellationToken cancellationToken = default)
        where TInstance : ProductInstance;

    /// <summary>
    /// Updates the database from the product instance
    /// </summary>
    Task SaveInstanceAsync(ProductInstance productInstance, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the database from the product instance
    /// </summary>
    Task SaveInstancesAsync(ProductInstance[] productInstances, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get instances with the given ids.
    /// </summary>
    /// <param name="ids">The IDs of instances that should be loaded</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <returns>The instance with the id when it exists.</returns>
    Task<IReadOnlyList<ProductInstance>> LoadInstancesAsync(long[] ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all instances that match a certain
    /// </summary>
    Task<IReadOnlyList<TInstance>> LoadInstancesAsync<TInstance>(Expression<Func<TInstance, bool>> selector, CancellationToken cancellationToken = default)
        where TInstance : ProductInstance;

    /// <summary>
    /// Load types using filter expression
    /// </summary>
    Task<IReadOnlyList<TType>> LoadTypesAsync<TType>(Expression<Func<TType, bool>> selector, CancellationToken cancellationToken = default);

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
    Task<bool> DeleteTypeAsync(long productTypeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove Recipe by given id
    /// </summary>
    Task DeleteRecipeAsync(long recipeId, CancellationToken cancellationToken = default);

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