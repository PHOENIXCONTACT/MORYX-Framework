using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Marvin.Model
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
        public virtual T GetByKey(long id)
        {
            return DbSet.FirstOrDefault(e => e.Id == id);
        }

        /// <inheritdoc />
        public ICollection<T> GetAll()
        {
            return DbSet.ToList();
        }

        /// <inheritdoc />
        public virtual ICollection<T> GetByKeys(long[] ids)
        {
            return DbSet.Where(e => ids.Contains(e.Id)).ToList();
        }

        /// <inheritdoc />
        public T Create()
        {
            return Create(true);
        }

        /// <inheritdoc />
        public T Create(bool addToContext)
        {
            var newInstance = DbSet.Create();
            if (addToContext)
                DbSet.Add(newInstance);

            return newInstance;
        }

        /// <inheritdoc />
        public void Remove(T entity)
        {
            Remove(entity, false);
        }

        /// <inheritdoc />
        public void Remove(T entity, bool permanent)
        {
            if (entity == null)
                return;

            ExecuteRemove(entity, permanent);
        }

        /// <see cref="IRepository{T}"/>
        public void RemoveRange(IEnumerable<T> entities)
        {
            RemoveRange(entities, false);   
        }

        /// <see cref="IRepository{T}"/>
        public void RemoveRange(IEnumerable<T> entities, bool permanent)
        {
            if (entities == null)
                return;

            ExecuteRemoveRange(entities, permanent);
        }

        /// <summary>
        /// Remove entity with option of permanent removal.
        /// </summary>
        protected virtual void ExecuteRemove(T entity, bool permanent)
        {
            DbSet.Remove(entity);
        }

        /// <summary>
        /// Remove entities with option of permanent removal.
        /// </summary>
        protected virtual void ExecuteRemoveRange(IEnumerable<T> entities, bool permanent)
        {
            DbSet.RemoveRange(entities.ToArray());
        }
	}

    /// <summary>
    /// Base class for entity framework repositories of modification tracking entities
    /// </summary>
    public abstract class ModificationTrackedRepository<T> : Repository<T>
        where T : class, IModificationTrackedEntity
    {
        /// <inheritdoc />
        protected override void ExecuteRemove(T entity, bool permanent)
        {
            if (permanent)
                base.ExecuteRemove(entity, true);
            else
                entity.Deleted = DateTime.Now;
        }

        /// <inheritdoc />
        protected override void ExecuteRemoveRange(IEnumerable<T> entities, bool permanent)
        {
            if (permanent)
                base.ExecuteRemoveRange(entities, true);
            else
            {
                foreach (var entity in entities)
                    entity.Deleted = DateTime.Now;
            }
        }
    }
}