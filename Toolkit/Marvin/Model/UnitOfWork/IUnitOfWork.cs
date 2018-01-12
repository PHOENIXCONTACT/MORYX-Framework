using System;

namespace Marvin.Model
{
    /// <summary>
    /// Unit of work for db access with specialized generic apis
    /// </summary>
    public interface IGenericUnitOfWork
    {
        /// <summary>
        /// Creates an repository by the given API type
        /// </summary>
        IRepository GetRepository(Type api);
    }

    /// <summary>
    /// Unit of work for db access
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Call to internal container for repo implementation
        /// </summary>
        T GetRepository<T>() where T : class, IRepository;

        /// <summary>
        /// Get or set the current mode
        /// </summary>
        ContextMode Mode { get; set; }

        /// <summary>
        /// Save all changes
        /// </summary>
        void Save();
    }
}
