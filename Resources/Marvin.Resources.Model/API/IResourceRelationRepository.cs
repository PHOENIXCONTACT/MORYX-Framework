using Marvin.Model;

namespace Marvin.Resources.Model
{
    /// <summary>
    /// The public API of the ResourceRelation repository.
    /// </summary>
    public interface IResourceRelationRepository : IRepository<ResourceRelation>
    {
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        ResourceRelation Create(int relationType);
    }
}