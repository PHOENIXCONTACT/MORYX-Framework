// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Data.Entity;

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
        /// Get or create an entity for a business object
        /// </summary>
        /// <param name="dbContext">An open database context</param>
        /// <param name="obj">The business object</param>
        /// <typeparam name="TEntity">The entity type to use</typeparam>
        public static TEntity GetEntity<TEntity>(this DbContext dbContext, IPersistentObject obj)
            where TEntity : class, IEntity
        {
            return dbContext.Set<TEntity>().GetEntity(obj);
        }
    }
}