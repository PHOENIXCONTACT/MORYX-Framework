// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Moryx.Model.Repositories
{
    /// <summary>
    /// Base class for entity framework repositories
    /// </summary>
    public abstract class Repository : IRepository
    {
        /// <inheritdoc />
        public IUnitOfWork UnitOfWork { get; private set; }

        /// <summary>
        /// The database context
        /// </summary>
        protected DbContext Context { get; set; }

        /// <summary>
        /// Initializes this repository with the responsible unit of work and the database context
        /// </summary>
        public virtual void Initialize(IUnitOfWork uow, DbContext context)
        {
            UnitOfWork = uow;
            Context = context;
        }
    }

    /// <summary>
    /// Base class for entity framework repositories
    /// </summary>
    public abstract class Repository<T> : Repository, IRepository<T>
        where T : class, IEntity, new()
    {
        /// <summary>
        /// Internal entity framework <see cref="DbSet{TEntity}"/>
        /// </summary>
        protected DbSet<T> DbSet { get; set; }

        /// <inheritdoc />
        public override void Initialize(IUnitOfWork uow, DbContext context)
        {
            base.Initialize(uow, context);
            DbSet = context.Set<T>();
        }

        /// <inheritdoc />
        public virtual IQueryable<T> Linq => DbSet;

        /// <inheritdoc />
        public virtual T GetByKey(long id) =>
            DbSet.FirstOrDefault(e => e.Id == id);

        /// <inheritdoc />
        public virtual Task<T> GetByKeyAsync(long id) =>
            DbSet.FirstOrDefaultAsync(e => e.Id == id);

        /// <inheritdoc />
        public ICollection<T> GetAll() =>
            DbSet.ToList();

        /// <inheritdoc />
        public virtual ICollection<T> GetByKeys(long[] ids) =>
            DbSet.Where(e => ids.Contains(e.Id)).ToList();

        /// <inheritdoc />
        public T Create() =>
            Create(true);

        /// <inheritdoc />
        public T Create(bool addToContext)
        {
            var newInstance = new T();
            if (addToContext)
                DbSet.Add(newInstance);

            return newInstance;
        }

        /// <inheritdoc />
        public T Add(T entityToAdd)
        {
            var entry = DbSet.Add(entityToAdd);
            return entry.Entity;
        }

        /// <inheritdoc />
        public IEnumerable<T> AddRange(IEnumerable<T> entitiesToAdd)
        {
            var entries = entitiesToAdd.ToList();
            DbSet.AddRange(entries);
            return entries;
        }

        /// <inheritdoc />
        public T Remove(T entity) =>
            Remove(entity, false);

        /// <inheritdoc />
        public T Remove(T entity, bool permanent) =>
            entity == null ? null : ExecuteRemove(entity, permanent);

        /// <see cref="IRepository{T}"/>
        public IEnumerable<T> RemoveRange(IEnumerable<T> entities) =>
            RemoveRange(entities, false);

        /// <see cref="IRepository{T}"/>
        public IEnumerable<T> RemoveRange(IEnumerable<T> entities, bool permanent) =>
            entities == null ? null : ExecuteRemoveRange(entities, permanent);

        /// <summary>
        /// Remove entity with option of permanent removal.
        /// </summary>
        protected virtual T ExecuteRemove(T entity, bool permanent)
        {
            var entityEntry = DbSet.Remove(entity);
            return entityEntry.Entity;
        }

        /// <summary>
        /// Remove entities with option of permanent removal.
        /// </summary>
        protected virtual IEnumerable<T> ExecuteRemoveRange(IEnumerable<T> entities, bool permanent)
        {
            var entityLst = entities.ToList();

            DbSet.RemoveRange(entityLst);
            return entityLst;
        }
    }

    /// <summary>
    /// Base class for entity framework repositories of modification tracking entities
    /// </summary>
    public abstract class ModificationTrackedRepository<T> : Repository<T>
        where T : class, IModificationTrackedEntity, new()
    {
        /// <inheritdoc />
        protected override T ExecuteRemove(T entity, bool permanent)
        {
            return permanent ? base.ExecuteRemove(entity, true) : DbSet.RemoveSoft(entity);
        }

        /// <inheritdoc />
        protected override IEnumerable<T> ExecuteRemoveRange(IEnumerable<T> entities, bool permanent)
        {
            if (permanent)
                return base.ExecuteRemoveRange(entities, true);

            var list = entities.ToArray();

            foreach (var entity in list)
                DbSet.RemoveSoft(entity);

            return list;
        }
    }
}