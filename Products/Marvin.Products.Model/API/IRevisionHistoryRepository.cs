using Marvin.Model;

namespace Marvin.Products.Model
{
    /// <summary>
    /// The public API of the RevisionHistory repository.
    /// </summary>
    public interface IRevisionHistoryRepository : IRepository<RevisionHistory>
    {
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        RevisionHistory Create(string comment); 
    }
}
