using Marvin.Model;

namespace Marvin.Products.Model
{
    /// <summary>
    /// The public API of the PartLink repository.
    /// </summary>
    public interface IPartLinkRepository : IRepository<PartLink>
    {
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        PartLink Create(string propertyName); 
    }
}
