using System.Collections.Generic;
using Marvin.Model;

namespace Marvin.Products.Model
{
    /// <summary>
    /// The public API of the WorkplanEntity repository.
    /// </summary>
    public interface IWorkplanEntityRepository : IRepository<WorkplanEntity>
    {
		/// <summary>
        /// Get all WorkplanEntitys from the database
        /// </summary>
		/// <param name="deleted">Include deleted entities in result</param>
		/// <returns>A collection of entities. The result may be empty but not null.</returns>
        ICollection<WorkplanEntity> GetAll(bool deleted);
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        WorkplanEntity Create(string name, int version, int state); 
    }
}
