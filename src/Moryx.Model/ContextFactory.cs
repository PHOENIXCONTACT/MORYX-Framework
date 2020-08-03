using System;
using System.Collections.Generic;
using System.Data.Entity;
using Moryx.Container;
using Moryx.Model.Configuration;

namespace Moryx.Model
{
    /// <inheritdoc />
    public class ContextFactory<TContext> : IContextFactory<TContext>
        where TContext : DbContext
    {
        /// <summary>
        /// Injected manager that does the real work
        /// </summary>
        public IDbContextManager Manager { get; set; }

        private readonly Dictionary<Type, Type> _repositories = new Dictionary<Type, Type>();
        private readonly RepositoryProxyBuilder _proxyBuilder = new RepositoryProxyBuilder();

        /// <inheritdoc />
        public TContext Create()
        {
            return Manager.Create<TContext>();
        }

        /// <inheritdoc />
        public TContext Create(IDatabaseConfig config)
        {
            return Manager.Create<TContext>(config);
        }

        /// <inheritdoc />
        public TContext Create(ContextMode contextMode)
        {
            return Manager.Create<TContext>(contextMode);
        }

        /// <inheritdoc />
        public TContext Create(IDatabaseConfig config, ContextMode contextMode)
        {
            return Manager.Create<TContext>(config, contextMode);
        }

        /// <inheritdoc />
        public TRepository GetRepository<TRepository>(TContext context)
        {
            var apiType = typeof(TRepository);

            var implType = _repositories.ContainsKey(apiType)
                ? _repositories[apiType]
                : _repositories[apiType] = _proxyBuilder.Build(typeof(TRepository));

            return Instantiate<TRepository>(context, implType);
        }

        /// <inheritdoc />
        public TRepository GetRepository<TRepository, TImplementation>(TContext context)
        {
            return GetRepository<TRepository, TImplementation>(context, false);
        }

        /// <inheritdoc />
        public TRepository GetRepository<TRepository, TImplementation>(TContext context, bool noProxy)
        {
            var apiType = typeof(TRepository);

            var implType = _repositories.ContainsKey(apiType)
                ? _repositories[apiType]
                : _repositories[apiType] = noProxy ? typeof(TImplementation) : _proxyBuilder.Build(apiType, typeof(TImplementation));

            return Instantiate<TRepository>(context, implType);
        }

        private static TRepository Instantiate<TRepository>(DbContext dbContext, Type implType)
        {
            var repoInstance = (IInitializableRepository)Activator.CreateInstance(implType, dbContext);
            repoInstance.Initialize(dbContext);

            return (TRepository)repoInstance;
        }
    }
}