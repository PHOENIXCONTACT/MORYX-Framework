using System;
using System.Data.Entity;
using System.Linq;

namespace Moryx.Model
{
    /// <summary>
    /// Extensions to the EF DbContext
    /// </summary>
    public static class DbContextExtensions
    {
        /// <summary>
        /// Configures the database context to the given mode
        /// </summary>
        public static void SetContextMode(this DbContext dbContext, ContextMode mode)
        {
            dbContext.Configuration.ProxyCreationEnabled = mode.HasFlag(ContextMode.ProxyOnly);
            dbContext.Configuration.LazyLoadingEnabled = mode.HasFlag(ContextMode.LazyLoading);
            dbContext.Configuration.AutoDetectChangesEnabled = mode.HasFlag(ContextMode.ChangeTracking);
            dbContext.Configuration.ValidateOnSaveEnabled = true;
        }

        /// <summary>
        /// Returns the currently configured <see cref="ContextMode"/> from the context
        /// </summary>
        public static ContextMode GetContextMode(this DbContext dbContext)
        {
            var mode = ContextMode.AllOff;

            if (dbContext.Configuration.ProxyCreationEnabled)
                mode |= ContextMode.ProxyOnly;
            if (dbContext.Configuration.LazyLoadingEnabled)
                mode |= ContextMode.LazyLoading;
            if (dbContext.Configuration.AutoDetectChangesEnabled)
                mode |= ContextMode.ChangeTracking;

            return mode;
        }

        /// <summary>
        /// Creates an entity and also adds it to the context
        /// </summary>
        public static TEntity CreateAndAdd<TEntity>(this DbSet<TEntity> dbSet) where TEntity : class
        {
            var entity = dbSet.Create();
            dbSet.Add(entity);
            return entity;
        }

        /// <summary>
        /// Get or create an entity for a business object
        /// </summary>
        /// <param name="dbSet">An open database set</param>
        /// <param name="obj">The business object</param>
        /// <typeparam name="TEntity">The entity type to use</typeparam>
        public static TEntity GetEntity<TEntity>(this DbSet<TEntity> dbSet, IPersistentObject obj)
            where TEntity : class, IEntity
        {
            var entity = dbSet.FirstOrDefault(e => e.Id == obj.Id);
            if (entity != null)
                return entity;

            entity = dbSet.Create();
            dbSet.Add(entity);
            EntityIdListener.Listen(entity, obj);

            return entity;
        }
    }
}