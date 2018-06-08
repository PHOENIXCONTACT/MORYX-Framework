using Marvin.Model;

// ReSharper disable once CheckNamespace
namespace Marvin.Resources.Model
{
    /// <summary>
    /// The public API of the ResourceEntity repository.
    /// </summary>
    public interface IResourceEntityRepository : IRepository<ResourceEntity>
    {
        /// <summary>
        /// Get first ResourceEntity that matches the given parameter 
        /// or null if no match was found.
        /// </summary>
        /// <param name="name">Value the entity has to match</param>
        /// <returns>First matching ResourceEntity</returns>
        ResourceEntity GetByName(string name);

        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        ResourceEntity Create(string name, string type);
    }
}