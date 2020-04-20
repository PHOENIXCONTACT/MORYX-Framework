using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Modules;
using Moryx.Tools;

namespace Moryx.Model
{
    [InitializableKernelComponent(typeof(IDbContextFactory))]
    public class DbContextFactory : IDbContextFactory, IInitializable
    {
        #region Dependencies

        public IConfigManager ConfigManager { get; set; }

        public IModuleLogger ModuleLogger { get; set; }

        #endregion

        private ModelWrapper[] _knownModels;

        /// <inheritdoc />
        public IReadOnlyCollection<IModelConfigurator> Configurators =>  _knownModels.Select(km => km.Configurator).ToList();

        /// <inheritdoc />
        public void Initialize()
        {
            var dbContextTypes = ReflectionTool.GetPublicClasses(typeof(DbContext), delegate (Type type)
            {
                var modelAttr = type.GetCustomAttribute<ModelAttribute>();
                return modelAttr != null;
            });

            _knownModels = dbContextTypes
                .Select(dbContextType =>
                    new { DbContextType = dbContextType, ModelAttr = dbContextType.GetCustomAttribute<ModelAttribute>() })
                .Select(t =>
                {
                    var wrapper = new ModelWrapper();
                    wrapper.Name = t.ModelAttr.Name;
                    wrapper.DbContextType = t.DbContextType;
                    wrapper.Configurator = (IModelConfigurator)Activator.CreateInstance(t.ModelAttr.ConfiguratorType);
                    return wrapper;
                }).ToArray();

            foreach (var wrapper in _knownModels)
                wrapper.Configurator.Initialize(wrapper.DbContextType, ConfigManager, ModuleLogger);
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
                ? (TContext) configurator.CreateContext(config, contextMode)
                : (TContext) configurator.CreateContext(contextMode);
        }

        private class ModelWrapper
        {
            public string Name { get; set; }

            public Type DbContextType { get; set; }

            public IModelConfigurator Configurator { get; set; }
        }
    }
}