using Marvin.Model;
using Marvin.Workflows;

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
        OutputDescriptionEntity Create(int index, OutputType outputType, long mappingValue); 
    }
}
