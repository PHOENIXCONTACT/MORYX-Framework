using Marvin.Model;

namespace Marvin.Products.Model
{
    /// <summary>
    /// The public API of the OutputDescriptionEntity repository.
    /// </summary>
    public interface IOutputDescriptionEntityRepository : IRepository<OutputDescriptionEntity>
    {
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        OutputDescriptionEntity Create(int index, bool success, long mappingValue); 
    }
}
