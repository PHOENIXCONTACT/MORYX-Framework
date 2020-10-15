// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using Moryx.AbstractionLayer.Products;
using Moryx.Products.Management.Modification.Model;
using Moryx.Serialization;
using Moryx.Tools.Wcf;

namespace Moryx.Products.Management.Modification
{
    [ServiceContract]
    [ServiceVersion("1.1.2.0")]
    internal interface IProductInteraction
    {
        /// <summary>
        /// Customization of the application, e.g. RecipeCreation, Importers, ....
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "products/customization", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ProductCustomization GetCustomization();
        
        /// <summary>
        /// Gets all products by filter
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "products", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ProductModel[] GetProducts(ProductQuery query);

        /// <summary>
        /// Create a new instance of the given type
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "products/create", Method = WebRequestMethods.Http.Put,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ProductModel CreateProduct(string type);

        /// <summary>
        /// Get details of a product
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "products/details?id={id}", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ProductModel GetProductDetails(long id);

        /// <summary>
        /// Save changes to a product
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "products/save", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ProductModel SaveProduct(ProductModel instance);

        /// <summary>
        /// Create a new revision or copy of the product
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "products/duplicate?sourceId={sourceId}&identifier={identifier}&revisionNo={revisionNo}", Method = WebRequestMethods.Http.Put,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        DuplicateProductResponse DuplicateProduct(long sourceId, string identifier, short revisionNo);

        /// <summary>
        /// Try to delete a product
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "products?id={id}", Method = "DELETE",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        bool DeleteProduct(long id);

        /// <summary>
        /// Import new products
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "products/import", Method = WebRequestMethods.Http.Put,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ProductModel ImportProduct(ImportProductRequest importProductRequest);

        /// <summary>
        /// Update import parameters based on their current content
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "products/updateParameters", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        Entry UpdateParameters(UpdateParametersRequest updateParametersRequest);

        /// <summary>
        /// Get the recipe with this id
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "products/recipe?recipeId={recipeId}", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        RecipeModel GetRecipe(long recipeId);

        /// <summary>
        /// Get all recipes for the given product
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "products/recipes?productId={productId}", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        RecipeModel[] GetRecipes(long productId);

        /// <summary>
        /// Create a new recipe
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "products/recipe/create?recipeType={recipeType}", Method = WebRequestMethods.Http.Put,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        RecipeModel CreateRecipe(string recipeType);

        /// <summary>
        /// Saves a recipe
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "products/recipe/save", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        RecipeModel SaveRecipe(SaveRecipeRequest saveRecipeRequest);

        /// <summary>
        /// Get all workplans
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "products/workplans", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        WorkplanModel[] GetWorkplans();

        /// <summary>
        /// Provider name
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "products/recipe/providername", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        string GetRecipeProviderName();
    }
}
