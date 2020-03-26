// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using Marvin.AbstractionLayer;
using Marvin.Model;
using Marvin.Modules;
using Marvin.Products.Model;

namespace Marvin.Products.Management
{
    /// <summary>
    /// API for the application specific product storage
    /// </summary>
    public interface IProductStorage : IPlugin
    {
        /// <summary>
        /// Get products by query
        /// </summary>
        IReadOnlyList<IProductType> GetTypes(ProductQuery query);

        /// <summary>
        /// Load product instance by id
        /// </summary>
        IProductType LoadType(long id);

        /// <summary>
        /// Load product by identity. This method supports loading a products latest revision
        /// </summary>
        IProductType LoadType(ProductIdentity identity);

        /// <summary>
        /// Transform a given a type entity
        /// </summary>
        IProductType TransformType(IUnitOfWork context, ProductTypeEntity typeEntity, bool full);

        /// <summary>
        /// Save a type to the storage
        /// </summary>
        long SaveType(IProductType modifiedInstance);

        /// <summary>
        /// Get an instance with the given id.
        /// </summary>
        /// <param name="id">The id for the article which should be searched for.</param>
        /// <returns>The article with the id when it exists.</returns>
        ProductInstance LoadInstance(long id);

        /// <summary>
        /// Load instances using combined bit flags
        /// </summary>
        IEnumerable<ProductInstance> LoadInstances(int state);

        /// <summary>
        /// Updates the database from the instance
        /// </summary>
        void SaveInstances(ProductInstance[] productInstance);

        /// <summary>
        /// Loads a recipe from the storage
        /// </summary>
        IProductRecipe LoadRecipe(long recipeId);

        /// <summary>
        /// Loads all recipes from the storage.
        /// </summary>
        IReadOnlyList<IProductRecipe> LoadRecipes(long productId, RecipeClassification classification);

        /// <summary>
        /// Saves the recipe of the product
        /// </summary>
        long SaveRecipe(IProductRecipe recipe);

        /// <summary>
        /// Save multiple recipes at once
        /// </summary>
        void SaveRecipes(long productId, ICollection<IProductRecipe> recipes);
    }
}
