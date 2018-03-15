using System.Collections.Generic;
using Marvin.Model;

namespace Marvin.Products.Model
{
    /// <summary>
    /// The public API of the StepEntity repository.
    /// </summary>
    public interface IStepEntityRepository : IRepository<StepEntity>
    {
		/// <summary>
        /// Get all StepEntitys from the database
        /// </summary>
		/// <param name="deleted">Include deleted entities in result</param>
		/// <returns>A collection of entities. The result may be empty but not null.</returns>
        ICollection<StepEntity> GetAll(bool deleted);
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        StepEntity Create(long stepId, string name, string assembly, string nameSpace, string classname); 
    }
}
