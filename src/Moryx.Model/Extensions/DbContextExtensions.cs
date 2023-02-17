// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;

namespace Moryx.Model
{
    /// <summary>
    /// Extensions to the EF DbContext
    /// </summary>
    public static class DbContextExtensions
    {
        /// <summary>
        /// Get or create an entity for a business object
        /// </summary>
        /// <param name="dbContext">An open database context</param>
        /// <param name="obj">The business object</param>
        /// <typeparam name="TEntity">The entity type to use</typeparam>
        public static TEntity GetEntity<TEntity>(this DbContext dbContext, IPersistentObject obj)
            where TEntity : class, IEntity, new()
        {
            return dbContext.Set<TEntity>().GetEntity(obj);
        }
    }
}