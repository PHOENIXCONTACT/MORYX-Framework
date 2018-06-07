using Marvin.AbstractionLayer;
using Marvin.Products.Management.Importers;

using Marvin.Serialization;

namespace Marvin.Products.Management.Modification
{
    internal interface IProductConverter
    {
        ProductModel[] GetRootProducts();

        ProductModel GetProduct(long id);

        ProductModel ReleaseProduct(long id);

        ProductModel CreateRevision(long id, short revisionNo, string comment);

        ProductModel ImportProduct(string importerName, IImportParameters parameters);

        ProductModel[] DeleteProduct(long id);

        ProductModel Save(ProductModel product);

        RecipeModel GetRecipe(long recipeId);

        RecipeModel CreateRecipe(string recipeType);

        RecipeModel GetProductionRecipe(long productId, long workplanId);

        RecipeModel CreateProductionRecipe(long productId, long workplanId, string name);

        WorkplanModel CreateWorkplan(string workplanName);

        WorkplanModel[] GetWorkplans();

        WorkplanModel GetWorkplan(long workplanId);
    }
}