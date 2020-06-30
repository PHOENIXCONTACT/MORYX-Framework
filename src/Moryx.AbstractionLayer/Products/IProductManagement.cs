// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Marvin.AbstractionLayer.Recipes;
using Marvin.Workflows;

namespace Marvin.AbstractionLayer.Products
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
        IProductType LoadType(long id);

        /// <summary>
        /// Load product type by identity
        /// </summary>
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
        IDictionary<string, IImportParameters> Importers { get; }

        /// <summary>
        /// Import product types for the given parameters with the named importer
        /// </summary>
        IReadOnlyList<IProductType> ImportTypes(string importerName, IImportParameters parameters);

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
        /// <param name="productType">Product to instanciate</param>
        /// <returns>Unsaved instance</returns>
        ProductInstance CreateInstance(IProductType productType);

        /// <summary>
        /// Create an product instance of given product
        /// </summary>
        /// <param name="productType">Product type to instanciate</param>
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
        /// Updates the database from the product instance
        /// </summary>
        void SaveInstance(ProductInstance productInstance);

        /// <summary>
        /// Updates the database from the product instance
        /// </summary>
        void SaveInstances(ProductInstance[] productInstances);

        /// <summary>
        /// Gets a list of product instances by a given state
        /// </summary>
        IEnumerable<ProductInstance> GetInstances(ProductInstanceState state);

        /// <summary>
        /// Load product instances using combined bit flags
        /// </summary>
        IEnumerable<ProductInstance> GetInstances(int combinedState);
    }
}
