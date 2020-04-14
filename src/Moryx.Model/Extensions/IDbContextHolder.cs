using System.Data.Entity;

namespace Moryx.Model
{
    /// <summary>
    /// Used for <see cref="IUnitOfWork"/> implementations holding an DbContext
    /// </summary>
    public interface IDbContextHolder
    {
        /// <summary>
        /// Current DbContext
        /// </summary>
        DbContext DbContext { get; }
    }
}