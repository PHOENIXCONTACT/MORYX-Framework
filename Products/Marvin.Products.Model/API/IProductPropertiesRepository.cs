using Marvin.Model;

namespace Marvin.Products.Model
{
    /// <summary>
    /// The public API of the ProductProperties repository.
    /// </summary>
    public interface IProductPropertiesRepository : IRepository<ProductProperties>
    {
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        ProductProperties Create(string name); 
    }
}
