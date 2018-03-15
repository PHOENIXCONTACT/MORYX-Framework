using System.Collections.Generic;
using Marvin.Model;

namespace Marvin.Products.Model
{
    /// <summary>
    /// The public API of the ProductProperties repository.
    /// </summary>
    public interface IProductPropertiesRepository : IRepository<ProductProperties>
    {
		/// <summary>
        /// Get all ProductPropertiess from the database
        /// </summary>
		/// <param name="deleted">Include deleted entities in result</param>
		/// <returns>A collection of entities. The result may be empty but not null.</returns>
        ICollection<ProductProperties> GetAll(bool deleted);
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        ProductProperties Create(string name); 
    }
}
