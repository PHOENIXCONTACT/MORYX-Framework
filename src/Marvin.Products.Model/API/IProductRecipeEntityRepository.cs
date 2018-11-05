using Marvin.Model;

namespace Marvin.Products.Model
{
    /// <summary>
    /// The public API of the ProductRecipeEntity repository.
    /// </summary>
    public interface IProductRecipeEntityRepository : IRepository<ProductRecipeEntity>
    {
        ProductRecipeEntity Create(string name, int classification, int state);
    }
}
