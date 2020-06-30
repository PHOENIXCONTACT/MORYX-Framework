// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ServiceModel;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Products;
using Moryx.Serialization;
using Moryx.Tools.Wcf;

namespace Moryx.Products.Management.Modification
{
    [ServiceContract]
    [ServiceVersion(ServerVersion = "1.1.2.0", MinClientVersion = "1.1.1.0")]
    internal interface IProductInteraction
    {
        /// <summary>
        /// Customization of the application, e.g. RecipeCreation, Importers, ....
        /// </summary>
        [OperationContract]
        ProductCustomization GetCustomization();

        /// <summary>
        /// Gets all products by filter
        /// </summary>
        [OperationContract]
        ProductModel[] GetProducts(ProductQuery query);

        /// <summary>
        /// Create a new instance of the given type
        /// </summary>
        [OperationContract]
        ProductModel CreateProduct(string type);

        /// <summary>
        /// Get details of a product
        /// </summary>
        [OperationContract]
        ProductModel GetProductDetails(long id);

        /// <summary>
        /// Save changes to a product
        /// </summary>
        [OperationContract]
        ProductModel SaveProduct(ProductModel instance);

        /// <summary>
        /// Create a new revision or copy of the product
        /// </summary>
        [OperationContract]
        DuplicateProductResponse DuplicateProduct(long sourceId, string identifier, short revisionNo);

        /// <summary>
        /// Try to delete a product
        /// </summary>
        [OperationContract]
        bool DeleteProduct(long id);

        /// <summary>
        /// Import new products
        /// </summary>
        [OperationContract]
        ProductModel ImportProduct(string importerName, Entry parametersModel);

        /// <summary>
        /// Update import parameters based on their current content
        /// </summary>
        [OperationContract]
        Entry UpdateParameters(string importer, Entry currentParameters);

        /// <summary>
        /// Get the recipe with this id
        /// </summary>
        [OperationContract]
        RecipeModel GetRecipe(long recipeId);

        /// <summary>
        /// Get all recipes for the given product
        /// </summary>
        [OperationContract]
        RecipeModel[] GetRecipes(long productId);

        /// <summary>
        /// Create a new recipe
        /// </summary>
        [OperationContract]
        RecipeModel CreateRecipe(string recipeType);

        /// <summary>
        /// Saves a recipe
        /// </summary>
        [OperationContract]
        RecipeModel SaveRecipe(RecipeModel recipe);

        /// <summary>
        /// Get all workplans
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        WorkplanModel[] GetWorkplans();

        /// <summary>
        /// Provider name
        /// </summary>
        [OperationContract]
        string GetRecipeProviderName();
    }
}
