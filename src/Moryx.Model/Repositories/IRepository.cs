// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;

namespace Moryx.Model.Repositories
{
    /// <summary>
    /// Base contract for all repositories defining UnitOfWork property
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Unit of work this repository controls access to
        /// </summary>
        IUnitOfWork UnitOfWork { get; }
    }

    /// <summary>
    /// Generic base repository for typed entity access
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    public interface IRepository<T> : IRepository where T : class
    {
        /// <summary>
        /// Entity framework db set <see cref="DbSet{TEntity}"/>
        /// </summary>
        DbSet<T> DbSet { get; }

        /// <summary>
        /// Linq queryable collection
        /// </summary>
        IQueryable<T> Linq { get; }

        /// <summary>
        /// Get the entity with this key
        /// </summary>
        T GetByKey(long id);

        /// <summary>
        /// Gets the entity with this key asynchronous
        /// </summary>
        Task<T> GetByKeyAsync(long id);

        /// <summary>
        /// Get all entities from the database
        /// </summary>
        /// <returns>A collection of entities. The result may be empty but not null.</returns>
        ICollection<T> GetAll();

        /// <summary>
        /// Get all entities from the database asynchronous
        /// </summary>
        /// <returns>A collection of entities. The result may be empty but not null.</returns>
        Task<ICollection<T>> GetAllAsync();

        /// <summary>
        /// Get all entities with this ids
        /// </summary>
        ICollection<T> GetByKeys(long[] ids);

        /// <summary>
        /// Get all entities with this ids asynchronous
        /// </summary>
        Task<ICollection<T>> GetByKeysAsync(long[] ids);

        /// <summary>
        /// Create a new instance of this type and add it to the context
        /// </summary>
        T Create();

        /// <summary>
        /// Create a new instance of this type and add it to the context
        /// </summary>
        Task<T> CreateAsync();

        /// <summary>
        /// Create a new instance but specify, if it should be added to the context
        /// </summary>
        /// <param name="addToContext"><value>True</value>Instance is added to the context. <value>False</value>Instance is not added to the context</param>
        T Create(bool addToContext);

        /// <summary>
        /// Create a new instance but specify, if it should be added to the context
        /// </summary>
        /// <param name="addToContext"><value>True</value>Instance is added to the context. <value>False</value>Instance is not added to the context</param>
        Task<T> CreateAsync(bool addToContext);

        /// <summary>
        /// Adds an entity to the context
        /// </summary>
        /// <param name="entity">Entity that shall be added</param>
        T Add(T entity);

        /// <summary>
        /// Adds an entity to the context
        /// </summary>
        /// <param name="entity">Entity that shall be added</param>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Add multiple entities to to context at one time
        /// </summary>
        /// <param name="entities">Entities that shall be added</param>
        IEnumerable<T> AddRange(IEnumerable<T> entities);

        /// <summary>
        /// Add multiple entities to to context at one time
        /// </summary>
        /// <param name="entities">Entities that shall be added</param>
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Remove entity. ModificationTracked entities will only update the
        /// Deleted flag.
        /// </summary>
        T Remove(T entity);

        /// <summary>
        /// Remove the entity and specifiy whether to remove permanent or
        /// simply flag it as deleted if possible.
        /// </summary>
        T Remove(T entity, bool permanent);

        /// <summary>
        /// Remove a range of entities from the context
        /// </summary>
        IEnumerable<T> RemoveRange(IEnumerable<T> entities);

        /// <summary>
        /// Remove a range of entities permanent
        /// </summary>
        IEnumerable<T> RemoveRange(IEnumerable<T> entities, bool permanent);
    }
}
