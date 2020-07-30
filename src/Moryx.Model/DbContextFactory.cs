using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Model.Configuration;
using Moryx.Modules;
using Moryx.Tools;

namespace Moryx.Model
{
    /// <summary>
    /// Kernel component handling data models and their runtime configurators
    /// </summary>
    [InitializableKernelComponent(typeof(IDbContextFactory))]
    public class DbContextFactory : IDbContextFactory, IInitializable, ILoggingHost
    {
        #region Dependencies

        public IConfigManager ConfigManager { get; set; }

        public IModuleLogger Logger { get; set; }

        public ILoggerManagement LoggerManagement { get; set; }

        #endregion

        string ILoggingHost.Name => nameof(DbContextFactory);
        private ModelWrapper[] _knownModels;

        /// <inheritdoc />
        public IReadOnlyCollection<IModelConfigurator> Configurators => _knownModels.Select(km => km.Configurator).ToList();

        /// <inheritdoc />
        public void Initialize()
        {
            LoggerManagement.ActivateLogging(this);

            var dbContextTypes = ReflectionTool.GetPublicClasses(typeof(DbContext), delegate (Type type)
            {
                var modelAttr = type.GetCustomAttribute<ModelConfiguratorAttribute>();
                return modelAttr != null;
            });

            _knownModels = dbContextTypes
                .Select(dbContextType => new
                {
                    DbContextType = dbContextType,
                    ModelConfiguratorAttr = dbContextType.GetCustomAttribute<ModelConfiguratorAttribute>()
                }).Select(t =>
                {
                    var wrapper = new ModelWrapper
                    {
                        DbContextType = t.DbContextType,
                        Configurator = (IModelConfigurator) Activator.CreateInstance(t.ModelConfiguratorAttr.ConfiguratorType)
                    };
                    return wrapper;
                }).ToArray();

            foreach (var wrapper in _knownModels)
            {
                var configuratorType = wrapper.Configurator.GetType();
                var logger = Logger.GetChild(configuratorType.Name, configuratorType);
                wrapper.Configurator.Initialize(wrapper.DbContextType, ConfigManager, logger);
            }
        }

        /// <inheritdoc />
        public TContext Create<TContext>() where TContext : DbContext =>
            Create<TContext>(ContextMode.AllOn);

        /// <inheritdoc />
        public TContext Create<TContext>(IDatabaseConfig config) where TContext : DbContext =>
            Create<TContext>(config, ContextMode.AllOn);

        /// <inheritdoc />
        public TContext Create<TContext>(ContextMode contextMode) where TContext : DbContext =>
            Create<TContext>(null, contextMode);

        /// <inheritdoc />
        public TContext Create<TContext>(IDatabaseConfig config, ContextMode contextMode) where TContext : DbContext
        {
            var wrapper = _knownModels.FirstOrDefault(k => k.DbContextType == typeof(TContext));
            if (wrapper == null)
                throw new InvalidOperationException("Unknown model");

            var configurator = wrapper.Configurator;

            return config != null
                ? (TContext)configurator.CreateContext(config, contextMode)
                : (TContext)configurator.CreateContext(contextMode);
        }

        private class ModelWrapper
        {
            public Type DbContextType { get; set; }

            public IModelConfigurator Configurator { get; set; }
        }
    }
}