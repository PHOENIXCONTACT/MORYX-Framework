using Marvin.Model;

namespace Marvin.Products.Model
{
    /// <summary>
    /// The public API of the WorkplanEntity repository.
    /// </summary>
    public interface IWorkplanEntityRepository : IRepository<WorkplanEntity>
    {
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        WorkplanEntity Create(string name, int version, int state); 
    }
}
