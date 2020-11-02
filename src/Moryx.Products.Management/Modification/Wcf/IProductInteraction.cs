// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using Moryx.AbstractionLayer.Products;
using Moryx.Serialization;
using Moryx.Tools.Wcf;

namespace Moryx.Products.Management.Modification
{
    [ServiceContract]
    [ServiceVersion("5.0.0")]
    internal interface IProductInteraction
    {
        /// <summary>
        /// Customization of the application, e.g. RecipeCreation, Importers, ....
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "customization", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ProductCustomization GetCustomization();
        
        /// <summary>
        /// Gets all products by filter
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "query", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ProductModel[] GetProducts(ProductQuery query);

        /// <summary>
        /// Create a new instance of the given type
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "construct/{type}", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ProductModel CreateProduct(string type);

        /// <summary>
        /// Get details of a product
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "product/{id}", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ProductModel GetProductDetails(string id);

        /// <summary>
        /// Save changes to a product
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "product/{id}", Method = WebRequestMethods.Http.Put,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ProductModel SaveProduct(string id, ProductModel instance);

        /// <summary>
        /// Create a new revision or copy of the product
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "product/{sourceId}/duplicate", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        DuplicateProductResponse DuplicateProduct(string sourceId, ProductModel model);

        /// <summary>
        /// Try to delete a product
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "product/{id}", Method = "DELETE",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        bool DeleteProduct(string id);

        /// <summary>
        /// Import new products
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "import/{importer}", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ImportStateModel Import(string importer, Entry importParameters);

        /// <summary>
        /// Poll progress of an import session
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "import/session/{guid}", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ImportStateModel FetchImportProgress(string guid);

        /// <summary>
        /// Update import parameters based on their current content
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "import/{importer}/parameters", Method = WebRequestMethods.Http.Put,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        Entry UpdateParameters(string importer, Entry importParameters);

        /// <summary>
        /// Get the recipe with this id
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "recipe/{recipeId}", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        RecipeModel GetRecipe(string recipeId);

        /// <summary>
        /// Get all recipes for the given product
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "recipes?product={productId}", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        RecipeModel[] GetRecipes(string productId);

        /// <summary>
        /// Create a new recipe
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "recipe/construct/{recipeType}", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        RecipeModel CreateRecipe(string recipeType);

        /// <summary>
        /// Saves a recipe
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "recipe", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        RecipeModel SaveRecipe(RecipeModel recipeModel);

        /// <summary>
        /// Saves a recipe
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "recipe/{id}", Method = WebRequestMethods.Http.Put,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        RecipeModel UpdateRecipe(string id, RecipeModel recipe);

        /// <summary>
        /// Get all workplans
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "workplans", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        WorkplanModel[] GetWorkplans();

        /// <summary>
        /// Provider name
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "recipe/provider", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        string GetRecipeProviderName();
    }
}
