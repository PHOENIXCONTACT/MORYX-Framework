using System.Data.Entity;

namespace Moryx.Model
{
    /// <summary>
    /// Extensions to the <see cref="IUnitOfWork"/>
    /// </summary>
    public static class UnitOfWorkExtensions
    {
        /// <summary>
        /// Returns the DbContext of the given <see cref="IUnitOfWork"/>
        /// </summary>
        public static DbContext GetDbContext(this IUnitOfWork unitOfWork) =>
            unitOfWork is IDbContextHolder contextHolder ? contextHolder.DbContext : null;
    }
}