using System;
using System.Collections.Generic;
using Marvin.Modules;

namespace Marvin.Model
{
    /// <summary>
    /// Base class for unit of work factories
    /// </summary>
    public abstract class UnitOfWorkFactoryBase<TContext> : IUnitOfWorkFactory, 
        IContextUnitOfWorkFactory, IInitializable, IDbContextFactory, IModelConfiguratorFactory
        where TContext : MarvinDbContext
    {
        private IModelConfigurator _configurator;
        private readonly IList<Type> _repositories = new List<Type>();
        private readonly RepositoryProxyBuilder _proxyBuilder;

        /// <summary>
        /// Creates a new instance of the <see cref="UnitOfWorkFactoryBase{TContext}"/>
        /// </summary>
        protected UnitOfWorkFactoryBase()
        {
            _proxyBuilder = new RepositoryProxyBuilder();
        }

        /// <inheritdoc />
        public void Initialize()
        {
            // Create configurator
            _configurator = CreateConfigurator();

            // Configure the factory
            Configure();
        }

        /// <summary>
        /// Creates the configurator for this model
        /// </summary>
        protected abstract IModelConfigurator CreateConfigurator();

        /// <summary>
        /// Will be called after initializing this instance. 
        /// Repositories should be added by <see cref="RegisterRepository{TApi}"/>
        /// </summary>
        protected abstract void Configure();

        /// <inheritdoc />
        public IUnitOfWork Create()
        {
            var context = CreateContext(ContextMode.AllOn);
            return Create(context);
        }

        /// <inheritdoc />
        public IUnitOfWork Create(ContextMode mode)
        {
            var context = CreateContext(mode);
            return Create(context);
        }

        /// <inheritdoc />
        IUnitOfWork IContextUnitOfWorkFactory.Create(MarvinDbContext context)
        {
            return Create(context);
        }
        
        internal IUnitOfWork Create(MarvinDbContext context)
        {
            return new UnitOfWork(context, _repositories);
        }

        /// <inheritdoc />
        IModelConfigurator IModelConfiguratorFactory.GetConfigurator()
        {
            return _configurator;
        }

        /// <inheritdoc />
        public MarvinDbContext CreateContext(ContextMode contextMode)
        {
            return CreateContext(typeof(TContext), contextMode);
        }

        /// <inheritdoc />
        public MarvinDbContext CreateContext(IDatabaseConfig config, ContextMode contextMode)
        {
            return (MarvinDbContext)Activator.CreateInstance(typeof(TContext), _configurator.BuildConnectionString(config), contextMode);
        }

        /// <summary>
        /// Registers an repository only by the api type. 
        /// A proxy class will be generated.
        /// </summary>
        protected void RegisterRepository<TApi>() 
            where TApi : IRepository
        {
            RegisterRepository(typeof(TApi));
        }

        /// <summary>
        /// Registers an repository with an custom implementation.
        /// For the implementation also a proxy class will be created.
        /// </summary>
        protected void RegisterRepository<TApi, TImpl>()
            where TApi : IRepository
            where TImpl : Repository
        {
            RegisterRepository(typeof(TApi), typeof(TImpl), false);
        }

        /// <summary>
        /// Registers an repository with an custom implementation.
        /// </summary>
        protected void RegisterRepository<TApi, TImpl>(bool noProxy)
            where TApi : IRepository
            where TImpl : Repository
        {
            RegisterRepository(typeof(TApi), typeof(TImpl), noProxy);
        }

        /// <summary>
        /// Registers an repository only by the api type. 
        /// A proxy class will be generated.
        /// </summary>
        private void RegisterRepository(Type apiType)
        {
            if (_repositories.Contains(apiType))
            {
                throw new ArgumentException("Repository interface already registered");
            }

            _repositories.Add(_proxyBuilder.Build(apiType));
        }

        /// <summary>
        /// Registers an repository with an custom implementation.
        /// For the implementation also a proxy class will be created.
        /// </summary>
        private void RegisterRepository(Type apiType, Type implType, bool noProxy)
        {
            var impl = noProxy ? implType : _proxyBuilder.Build(apiType, implType);

            if (_repositories.Contains(impl))
            {
                throw new ArgumentException("Repository interface already registered");
            }

            _repositories.Add(impl);
        }

        /// <summary>
        /// Creates an instance of the typed context
        /// </summary>
        protected virtual MarvinDbContext CreateContext(Type contextType, ContextMode contextMode)
        {
            var connectionString = _configurator.BuildConnectionString(_configurator.Config);
            return (MarvinDbContext)Activator.CreateInstance(contextType, connectionString, contextMode);
        }
    }
}