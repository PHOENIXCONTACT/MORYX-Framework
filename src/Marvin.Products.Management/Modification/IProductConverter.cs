using Marvin.AbstractionLayer;

namespace Marvin.Products.Management.Modification
{
    internal interface IProductConverter
    {
        ProductModel[] GetProducts(ProductQuery query);

        ProductModel Create(string type);

        ProductModel GetProduct(long id);

        DuplicateProductResponse Duplicate(long id, string identifier, short revisionNo);

        ProductModel ImportProduct(string importerName, IImportParameters parameters);

        bool DeleteProduct(long id);

        ProductModel Save(ProductModel product);

        RecipeModel GetRecipe(long recipeId);

        RecipeModel[] GetRecipes(long productId);

        RecipeModel CreateRecipe(string recipeType);

        RecipeModel SaveRecipe(RecipeModel recipe);

        WorkplanModel[] GetWorkplans();
    }
}