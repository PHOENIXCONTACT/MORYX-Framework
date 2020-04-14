// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Reflection;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Modules;

namespace Moryx.Model
{
    /// <summary>
    /// Base class for unit of work factories with possibility to by configured
    /// </summary>
    public abstract class UnitOfWorkFactoryBase<TContext, TConfigurator> : IUnitOfWorkFactory,
        IContextUnitOfWorkFactory, IInitializable, IDbContextFactory, IModelConfiguratorFactory,
        IParentFactory, IContainerChild<IUnitOfWorkFactory>, ILoggingHost where TContext : DbContext
        where TConfigurator : IModelConfigurator, new()
    {
        #region Dependencies

        /// <summary>
        /// Injection of a container to load children factories
        /// </summary>
        public IContainer Container { get; set; }

        /// <summary>
        /// Config manager to load configuration of the model
        /// </summary>
        public IConfigManager ConfigManager { get; set; }

        /// <summary>
        /// LoggerManagement to create Logger
        /// </summary>
        public ILoggerManagement LoggerManagement { get; set; }

        #endregion

        /// <inheritdoc />
        IUnitOfWorkFactory IContainerChild<IUnitOfWorkFactory>.Parent
        {
            get => _parent;
            set { /* This cannot be set from outside */ }
        }

        private IUnitOfWorkFactory _parent;
        private IModelConfigurator _configurator;
        private readonly IList<Type> _repositories = new List<Type>();
        private readonly RepositoryProxyBuilder _proxyBuilder;
        private readonly Dictionary<string, IUnitOfWorkFactory> _children = new Dictionary<string, IUnitOfWorkFactory>();

        string ILoggingHost.Name => "UnitOfWorkFactory";

        /// <summary>
        /// Root logger for the unitOfWorkFactory
        /// </summary>
        public IModuleLogger Logger { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="UnitOfWorkFactoryBase{TContext, TConfigurator}"/>
        /// </summary>
        protected UnitOfWorkFactoryBase()
        {
            _proxyBuilder = new RepositoryProxyBuilder();
        }

        /// <inheritdoc />
        public void Initialize()
        {
            // Activate logging (if LoggerManagement is available)
            LoggerManagement?.ActivateLogging(this);

            // Create configurator
            _configurator = new TConfigurator();
            _configurator.Initialize(this, ConfigManager, Logger);

            // Configure the factory
            Configure();

            // Register model as child if parent is set
            RegisterAsChild();
        }

        /// <summary>
        /// Will be called after initializing this instance.
        /// Repositories should be added by <see cref="RegisterRepository{TApi}"/>
        /// </summary>
        protected abstract void Configure();

        /// <summary>
        /// If the parent model is set,
        /// </summary>
        private void RegisterAsChild()
        {
            // Maybe a to do? I have no better idea
            // The parent model can be a protected virtual property but how to get the target model?
            var modelFactoryAttr = GetType().GetCustomAttribute<ModelFactoryAttribute>();
            if (string.IsNullOrEmpty(modelFactoryAttr?.ParentModel))
                return;

            var parentModel = Container?.Resolve<IUnitOfWorkFactory>(modelFactoryAttr.ParentModel);
            // ReSharper disable once UseNullPropagation -> Please resharper it has to be readable
            if (parentModel == null)
                return;

            // Register as child on the parent model
            _parent = parentModel;
            ((IParentFactory)parentModel).RegisterChild(this, modelFactoryAttr.TargetModel);
        }

        /// <inheritdoc />
        public IUnitOfWork Create()
        {
            if (_parent != null)
                return CreateMerged();

            var context = CreateContext(ContextMode.AllOn);
            return Create(context);
        }

        /// <inheritdoc />
        public IUnitOfWork Create(ContextMode mode)
        {
            if (_parent != null)
                return CreateMerged(mode);

            var context = CreateContext(mode);
            return Create(context);
        }

        /// <inheritdoc />
        IUnitOfWork IContextUnitOfWorkFactory.Create(DbContext context)
        {
            return Create(context);
        }

        internal IUnitOfWork Create(DbContext context)
        {
            return new UnitOfWork(context, _repositories);
        }

        internal IUnitOfWork CreateMerged()
        {
            return new MergedUnitOfWork(_parent.Create(ContextMode.AllOn), Create(CreateContext(ContextMode.AllOn)));
        }

        internal IUnitOfWork CreateMerged(ContextMode mode)
        {
            return new MergedUnitOfWork(_parent.Create(mode), Create(CreateContext(mode)));
        }

        /// <inheritdoc />
        IModelConfigurator IModelConfiguratorFactory.GetConfigurator() => _configurator;

        /// <inheritdoc />
        public DbContext CreateContext(ContextMode contextMode)
        {
            return CreateContext(typeof(TContext), contextMode);
        }

        /// <inheritdoc />
        public DbContext CreateContext(IDatabaseConfig config, ContextMode contextMode)
        {
            var dbContext = (DbContext)Activator.CreateInstance(typeof(TContext), _configurator.BuildConnectionString(config));
            dbContext.SetContextMode(contextMode);
            return dbContext;
        }

        /// <inheritdoc />
        IUnitOfWorkFactory INamedChildContainer<IUnitOfWorkFactory>.GetChild(string name, Type target)
        {
            if (string.IsNullOrEmpty(name))
                return this;

            return _children.ContainsKey(name) ? _children[name] : null;
        }

        /// <inheritdoc />
        void IParentFactory.RegisterChild(IUnitOfWorkFactory childFactory, string childModelName)
        {
            _children[childModelName] = childFactory;
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
                throw new ArgumentException($"Repository interface already registered: {apiType}");

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
                throw new ArgumentException($"Repository interface already registered: Api {apiType}, implementation {implType}, no proxy: {noProxy}");

            _repositories.Add(impl);
        }

        /// <summary>
        /// Creates an instance of the typed context
        /// </summary>
        protected virtual DbContext CreateContext(Type contextType, ContextMode contextMode)
        {
            var connectionString = _configurator.BuildConnectionString(_configurator.Config);
            var dbContext = (DbContext)Activator.CreateInstance(contextType, connectionString);
            dbContext.SetContextMode(contextMode);

            return dbContext;
        }
    }

    /// <summary>
    /// Base class for unit of work factories which cannot be configured
    /// </summary>
    public abstract class UnitOfWorkFactoryBase<TContext> : UnitOfWorkFactoryBase<TContext, NullModelConfigurator>
        where TContext : DbContext
    {

    }
}
