using Marvin.Model;

namespace Marvin.Products.Model
{
    /// <summary>
    /// The public API of the ProductEntity repository.
    /// </summary>
    public interface IProductEntityRepository : IRepository<ProductEntity>
    {
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        ProductEntity Create(string materialNumber, short revision, string typeName);

        /// <summary>
        /// This method returns the first matching ProductEntity for given parameters
        /// </summary>
        /// <param name="materialNumber">Value for MaterialNumber the ProductEntity has to match</param>
        /// <param name="revision">Value for Revision the ProductEntity has to match</param>
        ProductEntity GetByIdentity(string materialNumber, short revision);
    }
}
