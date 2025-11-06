// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Model.Attributes;
using Moryx.Model.Configuration;
using Moryx.Tools;

namespace Moryx.Model
{
    /// <summary>
    /// Kernel component handling data models and their runtime configurators
    /// </summary>
    public class DbContextManager : IDbContextManager
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfigManager _configManager;

        private readonly ConfiguredModelWrapper[] _configuredModels;
        private readonly PossibleModelWrapper[] _possibleModels;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextManager"/> class.
        /// </summary>
        /// <param name="configManager">Dependency to load model related configurations</param>
        /// <param name="loggerFactory">Logger factory to provide the <see cref="IModelConfigurator"/> a logger</param>
        public DbContextManager(IConfigManager configManager, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _configManager = configManager;

            _possibleModels = GetPossibleModels();
            _configuredModels = GetConfiguredModels();

            foreach (var wrapper in _configuredModels)
            {
                InitializeConfigurator(wrapper);
            }
        }

        /// <summary>
        /// This method uses reflection to find all possible models in the current AppDomain
        /// </summary>
        private static PossibleModelWrapper[] GetPossibleModels()
        {
            var possibleModels = ReflectionTool.GetPublicClasses(typeof(DbContext))
                .Where(type => type != typeof(DbContext) && typeof(DbContext).IsAssignableFrom(type) &&
                               !type.IsAbstract &&
                               type.GetCustomAttributes<DatabaseTypeSpecificDbContextAttribute>().Any())
                .SelectMany(type =>
                {
                    var dbTypeAttributes = type.GetCustomAttributes<DatabaseTypeSpecificDbContextAttribute>()!;
                    return dbTypeAttributes.Select(attr => new
                    {
                        DbContextType = type, BaseDbContextType = attr.BaseDbContextType ?? type, ModelConfiguratorType = attr.ModelConfiguratorType
                    });
                }).GroupBy(pc => pc.BaseDbContextType).Select(g =>
                {
                    var modelConfiguratorMap = g.ToDictionary(x => x.ModelConfiguratorType, x => x.DbContextType);
                    return new PossibleModelWrapper { DbContext = g.Key, ModelConfiguratorMap = modelConfiguratorMap };
                }).ToArray();

            return possibleModels;
        }

        /// <summary>
        /// This method loads all configured models with support of the <see cref="IConfigManager"/>
        /// It matches the configured models with the possible models and creates them
        /// </summary>
        private ConfiguredModelWrapper[] GetConfiguredModels()
        {
            var configuredModels = new List<ConfiguredModelWrapper>();
            foreach (var possibleModel in _possibleModels)
            {
                var config = _configManager.GetConfiguration<DatabaseConfig<DatabaseConnectionSettings>>(ConfigFilename(possibleModel.DbContext));
                Type configuratorType = null;
                Type specificDbContextType = null;
                if (!string.IsNullOrEmpty(config.ConfiguratorTypename))
                {
                    var configuredConfiguratorType = Type.GetType(config.ConfiguratorTypename);
                    if (configuredConfiguratorType != null &&
                        possibleModel.ModelConfiguratorMap.TryGetValue(configuredConfiguratorType, out specificDbContextType))
                    {
                        configuratorType = configuredConfiguratorType;
                    }
                }
                else
                {
                    var defaultMatch = possibleModel.ModelConfiguratorMap.FirstOrDefault();
                    if (!defaultMatch.Equals(default(KeyValuePair<Type, Type>)))
                    {
                        configuratorType = defaultMatch.Key;
                        specificDbContextType = defaultMatch.Value;
                    }
                }

                if (configuratorType == null || specificDbContextType == null)
                    throw new InvalidOperationException($"No valid configurator found for DbContext '{possibleModel.DbContext.FullName}'");

                var configType = configuratorType.BaseType.GenericTypeArguments.First();

                var typedConfig = (IDatabaseConfig)_configManager.GetConfiguration(configType,
                    ConfigFilename(possibleModel.DbContext), true);

                // If database is empty, fill with TargetModel name
                if (string.IsNullOrWhiteSpace(typedConfig.ConnectionSettings.Database))
                {
                    typedConfig.ConnectionSettings.Database = possibleModel.DbContext.Name;
                }

                configuredModels.Add(new ConfiguredModelWrapper
                {
                    BaseDbContextType = possibleModel.DbContext,
                    SpecificDbContextType = specificDbContextType,
                    DatabaseConfig = typedConfig,
                    Configurator = (IModelConfigurator)Activator.CreateInstance(configuratorType)
                });
            }

            return configuredModels.ToArray();
        }

        /// <inheritdoc />
        public void UpdateConfig(Type dbContextType, Type configuratorType, IDatabaseConfig databaseConfig)
        {
            _configManager.SaveConfiguration(databaseConfig, ConfigFilename(dbContextType));

            var modelWrapper = _configuredModels.First(w => w.BaseDbContextType == dbContextType);

            Type specificDbContextType = null;
            _possibleModels.FirstOrDefault(pm => pm.DbContext == dbContextType)?.ModelConfiguratorMap.TryGetValue(configuratorType, out specificDbContextType);
            modelWrapper.SpecificDbContextType = specificDbContextType;
            modelWrapper.Configurator = (IModelConfigurator)Activator.CreateInstance(configuratorType);
            modelWrapper.DatabaseConfig = databaseConfig;

            InitializeConfigurator(modelWrapper);
        }

        private void InitializeConfigurator(ConfiguredModelWrapper configuredModelWrapper)
        {
            var configuratorType = configuredModelWrapper.Configurator.GetType();
            var logger = _loggerFactory.CreateLogger(configuratorType);
            configuredModelWrapper.Configurator.Initialize(configuredModelWrapper.SpecificDbContextType, configuredModelWrapper.DatabaseConfig, logger);
        }

        private static string ConfigFilename(Type dbContextType)
            => dbContextType.FullName + ".DbConfig";

        /// <inheritdoc />
        public IReadOnlyCollection<Type> Contexts => _configuredModels.Select(km => km.BaseDbContextType).ToArray();

        /// <inheritdoc />
        public IModelConfigurator GetConfigurator(Type contextType) =>
            _configuredModels.First(km => km.BaseDbContextType == contextType).Configurator;

        /// <inheritdoc />
        public Type[] GetConfigurators(Type contextType)
        {
            return _possibleModels.FirstOrDefault(pm => pm.DbContext == contextType)?.ModelConfiguratorMap.Keys.ToArray();
        }

        /// <inheritdoc />
        public IModelSetupExecutor GetSetupExecutor(Type contextType)
        {
            var configuredContext = _configuredModels.FirstOrDefault(m => m.BaseDbContextType == contextType);
            if (configuredContext == null)
                throw new InvalidOperationException($"Context {contextType.FullName} not configured!");

            var setupExecutorType = typeof(ModelSetupExecutor<>).MakeGenericType(configuredContext.BaseDbContextType);
            return (IModelSetupExecutor)Activator.CreateInstance(setupExecutorType, this);
        }

        /// <inheritdoc />
        public TContext Create<TContext>() where TContext : DbContext =>
            Create<TContext>(null);

        /// <inheritdoc />
        public TContext Create<TContext>(IDatabaseConfig config) where TContext : DbContext
        {
            var wrapper = _configuredModels.FirstOrDefault(k => k.BaseDbContextType == typeof(TContext));
            if (wrapper == null)
                throw new InvalidOperationException("Unknown model");

            var configurator = wrapper.Configurator;

            return config != null
                ? (TContext)configurator.CreateContext(config)
                : (TContext)configurator.CreateContext();
        }

        private class ConfiguredModelWrapper
        {
            public Type BaseDbContextType { get; set; }

            public Type SpecificDbContextType { get; set; }

            public IModelConfigurator Configurator { get; set; }

            public IDatabaseConfig DatabaseConfig { get; set; }
        }

        private class PossibleModelWrapper
        {
            public Type DbContext { get; set; }

            public Dictionary<Type, Type> ModelConfiguratorMap { get; set; }
        }
    }
}
