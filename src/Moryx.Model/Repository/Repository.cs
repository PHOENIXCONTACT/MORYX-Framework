// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Moryx.Model
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
        where T : class, IEntity
    {
        /// <summary>
        /// Internal entity framework <see cref="IDbSet{TEntity}"/>
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
        public virtual Task<T> GetByKeyAsnyc(long id) =>
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
            var newInstance = DbSet.Create();
            if (addToContext)
                DbSet.Add(newInstance);

            return newInstance;
        }

        /// <inheritdoc />
        public T Add(T entityToAdd) =>
            DbSet.Add(entityToAdd);

        /// <inheritdoc />
        public IEnumerable<T> AddRange(IEnumerable<T> entitiesToAdd) =>
            DbSet.AddRange(entitiesToAdd);

        /// <inheritdoc />
        public T Remove(T entity) =>
            DbSet.Remove(entity);

        /// <see cref="IRepository{T}"/>
        public IEnumerable<T> RemoveRange(IEnumerable<T> entities) =>
             DbSet.RemoveRange(entities.ToArray());
    }
}
