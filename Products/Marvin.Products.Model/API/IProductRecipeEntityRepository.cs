using System.Collections.Generic;
using Marvin.Model;


namespace Marvin.Products.Model
{
    /// <summary>
    /// The public API of the ProductRecipeEntity repository.
    /// </summary>
    public interface IProductRecipeEntityRepository : IRepository<ProductRecipeEntity>
    {
		/// <summary>
        /// Get all ProductRecipeEntitys from the database
        /// </summary>
		/// <param name="deleted">Include deleted entities in result</param>
		/// <returns>A collection of entities. The result may be empty but not null.</returns>
        ICollection<ProductRecipeEntity> GetAll(bool deleted);
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        ProductRecipeEntity Create(string name, int classification, int state); 
    }
}
