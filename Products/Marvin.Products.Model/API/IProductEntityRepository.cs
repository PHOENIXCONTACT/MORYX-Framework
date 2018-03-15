using System.Collections.Generic;
using Marvin.Model;

namespace Marvin.Products.Model
{
    /// <summary>
    /// The public API of the ProductEntity repository.
    /// </summary>
    public interface IProductEntityRepository : IRepository<ProductEntity>
    {
        /// <summary>
        /// Get all ProductEntitys from the database
        /// </summary>
        /// <param name="deleted">Include deleted entities in result</param>
        /// <returns>A collection of entities. The result may be empty but not null.</returns>
        ICollection<ProductEntity> GetAll(bool deleted);
        /// <summary>
        /// Get first ProductEntity that matches the given parameter 
        /// or null if no match was found.
        /// </summary>
        /// <param name="materialNumber">Value the entity has to match</param>
        /// <returns>First matching ProductEntity</returns>
        ProductEntity GetByMaterialNumber(string materialNumber);
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
