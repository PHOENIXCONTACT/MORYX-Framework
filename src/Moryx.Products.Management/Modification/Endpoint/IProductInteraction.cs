// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.Communication.Endpoints;
using Moryx.Serialization;
#if USE_WCF
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using Moryx.Tools.Wcf;
#endif

namespace Moryx.Products.Management.Modification
{
#if USE_WCF
    [ServiceContract]
    [Endpoint(Name = nameof(IProductInteraction), Version = "5.0.0")]
#endif
    internal interface IProductInteraction
    {
        /// <summary>
        /// Customization of the application, e.g. RecipeCreation, Importers, ....
        /// </summary>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "customization", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        ProductCustomization GetCustomization();

        /// <summary>
        /// Gets all products by filter
        /// </summary>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "query", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        ProductModel[] GetProducts(ProductQuery query);

        /// <summary>
        /// Create a new instance of the given type
        /// </summary>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "construct/{type}", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        ProductModel CreateProduct(string type);

        /// <summary>
        /// Get details of a product
        /// </summary>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "product/{id}", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        ProductModel GetProductDetails(string id);

        /// <summary>
        /// Save changes to a product
        /// </summary>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "product/{id}", Method = WebRequestMethods.Http.Put,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        ProductModel SaveProduct(string id, ProductModel instance);

        /// <summary>
        /// Create a new revision or copy of the product
        /// </summary>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "product/{sourceId}/duplicate", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        DuplicateProductResponse DuplicateProduct(string sourceId, ProductModel model);

        /// <summary>
        /// Try to delete a product
        /// </summary>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "product/{id}", Method = "DELETE",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        bool DeleteProduct(string id);

        /// <summary>
        /// Import new products
        /// </summary>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "import/{importer}", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        ImportStateModel Import(string importer, Entry importParameters);

        /// <summary>
        /// Poll progress of an import session
        /// </summary>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "import/session/{guid}", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        ImportStateModel FetchImportProgress(string guid);

        /// <summary>
        /// Update import parameters based on their current content
        /// </summary>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "import/{importer}/parameters", Method = WebRequestMethods.Http.Put,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        Entry UpdateParameters(string importer, Entry importParameters);

        /// <summary>
        /// Get the recipe with this id
        /// </summary>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "recipe/{recipeId}", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        RecipeModel GetRecipe(string recipeId);

        /// <summary>
        /// Get all recipes for the given product
        /// </summary>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "recipes?product={productId}", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        RecipeModel[] GetRecipes(string productId);

        /// <summary>
        /// Create a new recipe
        /// </summary>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "recipe/construct/{recipeType}", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        RecipeModel CreateRecipe(string recipeType);

        /// <summary>
        /// Saves a recipe
        /// </summary>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "recipe", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        RecipeModel SaveRecipe(RecipeModel recipeModel);

        /// <summary>
        /// Saves a recipe
        /// </summary>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "recipe/{id}", Method = WebRequestMethods.Http.Put,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        RecipeModel UpdateRecipe(string id, RecipeModel recipe);

        /// <summary>
        /// Get all workplans
        /// </summary>
        /// <returns></returns>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "workplans", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        WorkplanModel[] GetWorkplans();

        /// <summary>
        /// Provider name
        /// </summary>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "recipe/provider", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        string GetRecipeProviderName();
    }
}
