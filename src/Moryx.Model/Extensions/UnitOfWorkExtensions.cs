// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Repositories;

namespace Moryx.Model
{
    /// <summary>
    /// A collection of static methods to be used with database repositories.
    /// </summary>
    public static class UnitOfWorkExtensions
    {
        /// <summary>
        /// Get or create an entity for a business object
        /// </summary>
        /// <param name="openContext">An open database context</param>
        /// <param name="obj">The business object</param>
        /// <typeparam name="TEntity">The entity type to use</typeparam>
        public static TEntity GetEntity<TEntity>(this IUnitOfWork openContext, IPersistentObject obj)
            where TEntity : class, IEntity
        {
            var dbSet = openContext.DbContext.Set<TEntity>();
            var entity = dbSet.GetByKey(obj.Id);

            if (entity == null)
            {
                entity = dbSet.Create();
                dbSet.Add(entity);
                EntityIdListener.Listen(entity, obj);
            }

            return entity;
        }
    }
}