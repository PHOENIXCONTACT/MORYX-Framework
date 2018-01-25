using System.Collections.Generic;
using Marvin.Model;

namespace Marvin.Resources.Model
{
    /// <summary>
    /// The public API of the ResourceEntity repository.
    /// </summary>
    public interface IResourceEntityRepository : IRepository<ResourceEntity>
    {
        /// <summary>
        /// Get all ResourceEntitys from the database
        /// </summary>
        /// <param name="deleted">Include deleted entities in result</param>
        /// <returns>A collection of entities. The result may be empty but not null.</returns>
        ICollection<ResourceEntity> GetAll(bool deleted);
        /// <summary>
        /// Get first ResourceEntity that matches the given parameter 
        /// or null if no match was found.
        /// </summary>
        /// <param name="name">Value the entity has to match</param>
        /// <returns>First matching ResourceEntity</returns>
        ResourceEntity GetByName(string name);
        /// <summary>
        /// Get first ResourceEntity that matches the given parameter 
        /// or null if no match was found.
        /// </summary>
        /// <param name="localIdentifier">Value the entity has to match</param>
        /// <returns>First matching ResourceEntity</returns>
        ResourceEntity GetByLocalIdentifier(string localIdentifier);
        /// <summary>
        /// Get first ResourceEntity that matches the given parameter 
        /// or null if no match was found.
        /// </summary>
        /// <param name="globalIdentifier">Value the entity has to match</param>
        /// <returns>First matching ResourceEntity</returns>
        ResourceEntity GetByGlobalIdentifier(string globalIdentifier);
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        ResourceEntity Create(string name, string type);
    }
}
