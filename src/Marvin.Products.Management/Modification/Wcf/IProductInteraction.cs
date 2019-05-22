using System.ServiceModel;
using Marvin.Products.Management.Importers;
using Marvin.Serialization;
using Marvin.Tools.Wcf;

namespace Marvin.Products.Management.Modification
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
        /// Update import parameters based on their current content
        /// </summary>
        [OperationContract]
        Entry UpdateParameters(string importer, Entry currentParameters);

        /// <summary>
        /// Get the product tree
        /// </summary>
        [OperationContract]
        ProductStructureEntry[] GetProductStructure();

        /// <summary>
        /// Get producible root products
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        ProductModel[] GetAllProducts();

        /// <summary>
        /// Get details of a product
        /// </summary>
        [OperationContract]
        ProductModel GetProductDetails(long id);

        /// <summary>
        /// Get all revisions of a product
        /// </summary>
        [OperationContract]
        ProductRevisionEntry[] GetProductRevisions(string identifier);

        /// <summary>
        /// Save changes to a product
        /// </summary>
        [OperationContract]
        ProductModel SaveProduct(ProductModel instance);

        /// <summary>
        /// Release the product
        /// </summary>
        [OperationContract]
        ProductModel ReleaseProduct(long id);

        /// <summary>
        /// Create a new revision of the product
        /// </summary>
        [OperationContract]
        ProductModel CreateRevision(long id, short revisionNo, string comment);

        /// <summary>
        /// Import new products
        /// </summary>
        [OperationContract]
        ProductModel ImportProduct(string importerName, Entry parametersModel);

        /// <summary>
        /// Try to delete a product
        /// </summary>
        [OperationContract]
        ProductModel[] DeleteProduct(long id);

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
        /// Get a recipe for this workplan
        /// </summary>
        [OperationContract]
        RecipeModel GetProductionRecipe(long productId, long workplanId);

        /// <summary>
        /// Create a recipe for this workplan with the given name
        /// </summary>
        [OperationContract]
        RecipeModel CreateProductionRecipe(long productId, long workplanId, string name);

        /// <summary>
        /// Saves a recipe
        /// </summary>
        [OperationContract]
        bool SaveProductionRecipe(RecipeModel recipe);

        /// <summary>
        /// Create a new workplan
        /// </summary>
        [OperationContract]
        WorkplanModel CreateWorkplan(string name);

        /// <summary>
        /// Get all workplans
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        WorkplanModel[] GetWorkplans();

        /// <summary>
        /// Get workplan with this id
        /// </summary>
        [OperationContract]
        WorkplanModel GetWorkplan(long id);

        /// <summary>
        /// Provider name
        /// </summary>
        [OperationContract]
        string GetRecipeProviderName();
    }
}