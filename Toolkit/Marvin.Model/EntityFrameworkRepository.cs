using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Marvin.Model
{
    /// <summary>
    /// Base class for entity framework repositories
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EntityFrameworkRepository<T> : IRepository<T> 
        where T : class, IEntity
    {
        /// <summary>
        /// Internal entity framework <see cref="IDbSet{TEntity}"/>
        /// </summary>
        protected DbSet<T> DbSet { get; set; }

        /// <summary>
        /// The database context 
        /// </summary>
        protected DbContext Context { get; set; }


        /// <see cref="IRepository"/>
        public IUnitOfWork UnitOfWork { get; set; }

        /// <see cref="IRepository{T}"/>
        public virtual IQueryable<T> Linq
        {
            get { return DbSet; }
        }

        /// <see cref="IRepository{T}"/>
        public virtual T GetByKey(long id)
        {
            return DbSet.FirstOrDefault(e => e.Id == id);
        }

        /// <see cref="IRepository{T}"/>
        public virtual ICollection<T> GetByKeys(long[] ids)
        {
            return DbSet.Where(e => ids.Contains(e.Id)).ToList();
        }

        /// <see cref="IRepository{T}"/>
        public virtual T Create()
        {
            return Create(true);
        }

        /// <see cref="IRepository{T}"/>
        public virtual T Create(bool addToContext)
        {
            var newInstance = DbSet.Create();
            if (addToContext)
                DbSet.Add(newInstance);

            return newInstance;
        }

        /// <see cref="IRepository{T}"/>
        public void Remove(T entity)
        {
            Remove(entity, false);
        }

        /// <see cref="IRepository{T}"/>
        public void Remove(T entity, bool permanent)
        {
            if (entity == null)
                return;

            ExecuteRemove(entity, permanent);
        }

        /// <summary>
        /// Remote entity with option of permanent removal.
        /// </summary>
        protected virtual void ExecuteRemove(T entity, bool permanent)
        {
            DbSet.Remove(entity);
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
        /// Remote entities with option of permanent removal.
        /// </summary>
        protected virtual void ExecuteRemoveRange(IEnumerable<T> entities, bool permanent)
        {
            DbSet.RemoveRange(entities.ToArray());
        }

        /// <see cref="IRepository{T}"/>
        public virtual void Load<TOut>(T entity, Expression<Func<T, TOut>> loadExpression) where TOut : class
        {
            Context.Entry(entity).Reference(loadExpression).Load();
        }

        /// <see cref="IRepository{T}"/>
        public virtual void Load<TOut>(T entity, Expression<Func<T, ICollection<TOut>>> loadExpression) where TOut : class
        {
            Context.Entry(entity).Collection(loadExpression).Load();
        }

        /// <see cref="IRepository{T}"/>
        public virtual void Load<TOut>(T entity, Expression<Func<T, ICollection<TOut>>> loadExpression, Expression<Func<TOut, bool>> filter) where TOut : class
        {
            Context.Entry(entity).Collection(loadExpression).Query().Where(filter).Load();
        }
	}
}
