using System.Collections.Generic;
using System.Linq;

namespace Marvin.Model
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
    public interface IRepository<T> : IRepository
    {
        /// <summary>
        /// Linq queryable collection
        /// </summary>
        IQueryable<T> Linq { get; }

        /// <summary>
        /// Get the entity with this key
        /// </summary>
        T GetByKey(long id);

        /// <summary>
        /// Get all entities from the database
        /// </summary>
        /// <returns>A collection of entities. The result may be empty but not null.</returns>
        ICollection<T> GetAll();

        /// <summary>
        /// Get all entities with this ids
        /// </summary>
        ICollection<T> GetByKeys(long[] ids); 

        /// <summary>
        /// Create a new instance of this type and add it to the context
        /// </summary>
        T Create();

        /// <summary>
        /// Create a new instance but specify, if it should be added to the context
        /// </summary>
        /// <param name="addToContext"><value>True</value>Instance is added to the context. <value>False</value>Instance is not added to the context</param>
        T Create(bool addToContext);

        /// <summary>
        /// Adds an entity to the context
        /// </summary>
        /// <param name="entityToAdd">Entity that shall be added</param>
        void Add(T entityToAdd);

        /// <summary>
        /// Add multiple entities to to context at one time
        /// </summary>
        /// <param name="entitiesToAdd">Entities that shall be added</param>
        void AddRange(IEnumerable<T> entitiesToAdd);

        /// <summary>
        /// Remove entity. ModificationTracked entities will only update the
        /// Deleted flag.
        /// </summary>
        void Remove(T entity);

        /// <summary>
        /// Remove the entity and specifiy whether to remove permanenent or 
        /// simply flag it as deleted if possible.
        /// </summary>
        void Remove(T entity, bool permanent);

        /// <summary>
        /// Remove a range of entities from the context
        /// </summary>
        void RemoveRange(IEnumerable<T> entities);

        /// <summary>
        /// Remove a range of entities permanent
        /// </summary>
        void RemoveRange(IEnumerable<T> entities, bool permanent);
    }
}
