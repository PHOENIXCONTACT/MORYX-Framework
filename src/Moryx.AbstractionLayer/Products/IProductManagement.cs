// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
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
        IReadOnlyList<IProductType> LoadTypes(ProductQuery query);

        /// <summary>
        /// Load product type by id
        /// </summary>
        /// <exception cref="ProductNotFoundException">Thrown when the product with the given id doesn't exist.</exception>
        IProductType LoadType(long id);

        /// <summary>
        /// Load product type by identity
        /// </summary>
        /// <exception cref="ProductNotFoundException">Thrown when the product with the given id doesn't exist.</exception>
        IProductType LoadType(ProductIdentity identity);

        /// <summary>
        /// Event raised when a product type changed
        /// </summary>
        event EventHandler<IProductType> TypeChanged;

        /// <summary>
        /// Duplicate a product under a new identity
        /// </summary>
        /// <exception cref="IdentityConflictException">Thrown when the new identity causes conflicts</exception>
        IProductType Duplicate(IProductType template, ProductIdentity newIdentity);

        /// <summary>
        /// Save a product type
        /// </summary>
        long SaveType(IProductType modifiedInstance);

        /// <summary>
        /// All importers and their parameters currently configured in the machine
        /// </summary>
        IDictionary<string, object> Importers { get; }

        /// <summary>
        /// Import product types for the given parameters with the named importer
        /// </summary>
        Task<ProductImportResult> Import(string importerName, object parameters);

        /// <summary>
        /// Retrieves the current recipe for this product
        /// </summary>
        IReadOnlyList<IProductRecipe> GetRecipes(IProductType productType, RecipeClassification classification);

        /// <summary>
        /// Saves given recipe to the storage
        /// </summary>
        long SaveRecipe(IProductRecipe recipe);

        /// <summary>
        /// Create an product instance of given product
        /// </summary>
        /// <param name="productType">Product to instantiate</param>
        /// <returns>Unsaved instance</returns>
        ProductInstance CreateInstance(IProductType productType);

        /// <summary>
        /// Create an product instance of given product
        /// </summary>
        /// <param name="productType">Product type to instantiate</param>
        /// <param name="save">Flag if new instance should already be saved</param>
        /// <returns>New instance</returns>
        ProductInstance CreateInstance(IProductType productType, bool save);

        /// <summary>
        /// Get an product instance with the given id.
        /// </summary>
        /// <param name="id">The id for the product instance which should be searched for.</param>
        /// <returns>The product instance with the id when it exists.</returns>
        ProductInstance GetInstance(long id);

        /// <summary>
        /// Get an instance with this identity
        /// </summary>
        ProductInstance GetInstance(IIdentity identity);

        /// <summary>
        /// Get only instances that match a certain condition, similar to SingleOrDefault
        /// </summary>
        TInstance GetInstance<TInstance>(Expression<Func<TInstance, bool>> selector)
            where TInstance : IProductInstance;

        /// <summary>
        /// Updates the database from the product instance
        /// </summary>
        void SaveInstance(ProductInstance productInstance);

        /// <summary>
        /// Updates the database from the product instance
        /// </summary>
        void SaveInstances(ProductInstance[] productInstances);

        /// <summary>
        /// Get instances with the given ids.
        /// </summary>
        /// <param name="ids">The IDs of instances that should be loaded</param>
        /// <returns>The instance with the id when it exists.</returns>
        IReadOnlyList<ProductInstance> GetInstances(long[] ids);

        /// <summary>
        /// Get all instances that match a certain 
        /// </summary>
        IReadOnlyList<TInstance> GetInstances<TInstance>(Expression<Func<TInstance, bool>> selector)
            where TInstance : IProductInstance;

        /// <summary>
        /// Load types using filter expression
        /// </summary>
        IReadOnlyList<TType> LoadTypes<TType>(Expression<Func<TType, bool>> selector);

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
        bool DeleteProduct(long id);

        /// <summary>
        /// Remove Recipe by given id
        /// </summary>
        void RemoveRecipe(long recipeId);

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
