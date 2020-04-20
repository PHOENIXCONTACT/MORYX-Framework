using Moryx.Model;

namespace Moryx.Tools
{
    /// <summary>
    /// A collection of static methods to be used with databae repositories.
    /// </summary>
    //public static class UnitOfWorkExtensions
    //{
    //    /// <summary>
    //    /// Get or create an entity for a business object
    //    /// </summary>
    //    /// <param name="openContext">An open database context</param>
    //    /// <param name="obj">The business object</param>
    //    /// <typeparam name="TEntity">The entity type to use</typeparam>
    //    public static TEntity GetEntity<TEntity>(this IUnitOfWork openContext, IPersistentObject obj) 
    //        where TEntity : class, IEntity 
    //    {
    //        var repo = openContext.GetRepository<IRepository<TEntity>>();
    //        var entity = repo.GetByKey(obj.Id);

    //        if (entity == null)
    //        {
    //            entity = repo.Create();
    //            EntityIdListener.Listen(entity, obj);
    //        }

    //        return entity;
    //    }
    //}
}