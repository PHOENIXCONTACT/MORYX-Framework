// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq.Expressions;

namespace Moryx.Model;

/// <summary>
/// Extensions for the <see cref="IQueryable{T}"/>
/// </summary>
public static class QueryableExtensions
{
    /// <param name="query">The current query</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    extension<TSource>(IQueryable<TSource> query) where TSource : class, IModificationTrackedEntity
    {
        /// <summary>
        /// Queries all entities which are not deleted
        /// </summary>
        /// <returns>Filtered query with no deleted items</returns>
        public IQueryable<TSource> Active()
        {
            return query.Where(element => element.Deleted == null);
        }

        /// <summary>
        /// Queries all entities wich are not deleted
        /// </summary>
        /// <param name="predicate">>A function to test each element for a condition.</param>
        /// <returns>Filtered query with no deleted items</returns>
        public IQueryable<TSource> Active(Expression<Func<TSource, bool>> predicate)
        {
            return query.Active().Where(predicate);
        }

        /// <summary>
        /// Queries only deleted entities.
        /// </summary>
        /// <returns>Deleted items</returns>
        public IQueryable<TSource> Deleted()
        {
            return query.Where(element => element.Deleted != null);
        }

        /// <summary>
        /// Queries only deleted entities.
        /// </summary>
        /// <param name="predicate">>A function to test each element for a condition.</param>
        /// <returns>Deleted items</returns>
        public IQueryable<TSource> Deleted(Expression<Func<TSource, bool>> predicate)
        {
            return query.Deleted().Where(predicate);
        }

        /// <summary>
        /// Queries all entities which are modififed since a specified date.
        /// </summary>
        /// <param name="date">The start date to query</param>
        /// <returns>Filtered query</returns>
        public IQueryable<TSource> ModifiedSince(DateTime date)
        {
            return ModifiedSince(query, date, false);
        }

        /// <summary>
        /// Queries all items which are modififed since a specified date.
        /// </summary>
        /// <param name="date">The start date to query</param>
        /// <param name="withDeleted">Determines if the query should contain deleted entities</param>
        /// <returns>Filtered query</returns>
        public IQueryable<TSource> ModifiedSince(DateTime date, bool withDeleted)
        {
            return query.Active().Where(e => e.Updated > date && ((e.Deleted == null) || withDeleted));
        }

        /// <summary>
        /// Queries all items which are created since a specified date.
        /// </summary>
        /// <param name="date">The start date to query</param>
        /// <returns>Filtered query</returns>
        public IQueryable<TSource> CreatedSince(DateTime date)
        {
            return CreatedSince(query, date, false);
        }

        /// <summary>
        /// Queries all items which are created since a specified date.
        /// </summary>
        /// <param name="date">The start date to query</param>
        /// <param name="withDeleted">Determines if the query should contain deleted entities</param>
        /// <returns>Filtered query</returns>
        public IQueryable<TSource> CreatedSince(DateTime date, bool withDeleted)
        {
            return query.Active().Where(e => e.Created > date && (e.Deleted != null) == withDeleted);
        }
    }

    /// <param name="query">The current query.</param>
    /// <typeparam name="TEntity">The type of the source.</typeparam>
    extension<TEntity>(IQueryable<TEntity> query) where TEntity : class, IEntity
    {
        /// <summary>
        /// Queries all items with the given keys.
        /// Keys which cannot be found will be ignored!
        /// </summary>
        /// <param name="keys">The keys to find</param>
        /// <returns>Filtered query</returns>
        public IQueryable<TEntity> ByKeys(long[] keys)
        {
            return query.Where(e => keys.Contains(e.Id));
        }

        /// <summary>
        /// Queries a single entity with the given key
        /// </summary>
        /// <param name="key">The key to find</param>
        /// <returns>Filtered query</returns>
        public TEntity GetByKey(long key)
        {
            return query.FirstOrDefault(e => e.Id == key);
        }
    }
}