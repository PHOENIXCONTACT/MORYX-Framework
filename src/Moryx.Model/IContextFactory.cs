using System.Data.Entity;
using Moryx.Model.Configuration;

namespace Moryx.Model
{
    /// <summary>
    /// Dedicated factory for a context
    /// </summary>
    public interface IContextFactory<TContext>
        where TContext : DbContext
    {
        /// <summary>
        /// Create context using standard mode and config from config manager
        /// </summary>
        TContext Create();

        /// <summary>
        /// Create context using standard mode and alternative config
        /// </summary>
        TContext Create(IDatabaseConfig config);

        /// <summary>
        /// Create context using given mode and config from config manager
        /// </summary>
        TContext Create(ContextMode contextMode);

        /// <summary>
        /// Create context using given mode and alternative config
        /// </summary>
        TContext Create(IDatabaseConfig config, ContextMode contextMode);

        /// <summary>
        /// Get repository for this context
        /// </summary>
        TRepository GetRepository<TRepository>(TContext context);

        /// <summary>
        /// Get repository for this context
        /// </summary>
        TRepository GetRepository<TRepository, TImplementation>(TContext context);

        /// <summary>
        /// Get repository for this context
        /// </summary>
        TRepository GetRepository<TRepository, TImplementation>(TContext context, bool noProxy);
    }
}