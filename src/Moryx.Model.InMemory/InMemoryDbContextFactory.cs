using System;
using System.Collections.Generic;
using System.Data.Entity;
using Moryx.Model.Configuration;

namespace Moryx.Model.InMemory
{
    /// <summary>
    /// In memory implementation of the <see cref="IDbContextFactory"/>
    /// </summary>
    public class InMemoryDbContextFactory : IDbContextFactory
    {
        private readonly string _instanceId;

        public IReadOnlyCollection<IModelConfigurator> Configurators => Array.Empty<IModelConfigurator>();

        public InMemoryDbContextFactory() : this(string.Empty)
        {
        }

        public InMemoryDbContextFactory(string instanceId)
        {
            _instanceId = instanceId;
        }

        /// <inheritdoc />
        public TContext Create<TContext>() where TContext : DbContext =>
            Create<TContext>(null);

        /// <inheritdoc />
        public TContext Create<TContext>(IDatabaseConfig config) where TContext : DbContext =>
            Create<TContext>(ContextMode.AllOn);

        /// <inheritdoc />
        public TContext Create<TContext>(ContextMode contextMode) where TContext : DbContext =>
            Create<TContext>(null, contextMode);

        /// <inheritdoc />
        public TContext Create<TContext>(IDatabaseConfig config, ContextMode contextMode) where TContext : DbContext
        {
            var connection = string.IsNullOrEmpty(_instanceId)
                ? Effort.DbConnectionFactory.CreatePersistent(Guid.NewGuid().ToString())
                : Effort.DbConnectionFactory.CreatePersistent(_instanceId);

            // Create instance of context
            var context = (TContext)Activator.CreateInstance(typeof(TContext), connection);

            // Override initializer of MoryxDbContext: Create database if not exists
            Database.SetInitializer(new CreateDatabaseIfNotExists<TContext>());

            return context;
        }
    }
}
