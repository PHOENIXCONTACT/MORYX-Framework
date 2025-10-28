// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Model.Attributes;
using Moryx.Model.Configuration;
using Moryx.Tools;

namespace Moryx.Model
{
    /// <summary>
    /// Kernel component handling data models and their runtime configurators
    /// </summary>
    [InitializableKernelComponent(typeof(IDbContextManager))]
    public class DbContextManager : IDbContextManager
    {
        private ModelWrapper[] _knownModels;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfigManager _configManager;
        private static readonly Type[] AllDbContextTypes;
        static DbContextManager()
        {
            AllDbContextTypes = ReflectionTool.GetPublicClasses(typeof(DbContext))
                .Where(type => type != typeof(DbContext) && typeof(DbContext).IsAssignableFrom(type)).ToArray();
        }

        /// <inheritdoc />
        public DbContextManager(IConfigManager configManager, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _configManager = configManager;

            var baseDbContextTypes = AllDbContextTypes
                .Where(type => !type.GetCustomAttributes<DatabaseSpecificContextAttribute>().Any());
            
            _knownModels = baseDbContextTypes
                .Select(dbContextType =>
                {
                    var config = configManager.GetConfiguration<DatabaseConfig<DatabaseConnectionSettings>>(ConfigFilename(dbContextType));
                    var configuratorType = !string.IsNullOrEmpty(config.ConfiguratorTypename)
                        ? Type.GetType(config.ConfiguratorTypename)
                        : DefaultConfigurator();

                    // Try to find specific DbContext for the configurator
                    // If no specific context found, use the base one
                    var specificDbContext = GetSpecificDbContext(dbContextType, configuratorType);

                    return new ModelWrapper
                    {
                        DbContextType = dbContextType,
                        SpecificDbContext = specificDbContext,
                        Configurator = (IModelConfigurator)Activator.CreateInstance(configuratorType)
                    };
                }).ToArray();

            foreach (var wrapper in _knownModels)
            {
                InitializeConfigurator(wrapper);
            }
        }

        // TODO: Reference to an assembly which might not be referencesd in certain setups
        private Type DefaultConfigurator()
        {
            var sqliteModelConfigurator = ReflectionTool.GetAssemblies()
                .FirstOrDefault(x => x.GetName().Name.Contains("Moryx.Model.Sqlite"))
                ?.GetTypes()
                .FirstOrDefault(x => x.Name == "SqliteModelConfigurator");

            return sqliteModelConfigurator ?? typeof(NullModelConfigurator);
        }

        /// <inheritdoc />
        public void UpdateConfig(Type dbContextType, Type configuratorType, IDatabaseConfig databaseConfig)
        {
            _configManager.SaveConfiguration(databaseConfig, ConfigFilename(dbContextType));

            var modelWrapper = _knownModels.First(w => w.DbContextType == dbContextType);
            modelWrapper.Configurator = (IModelConfigurator)Activator.CreateInstance(configuratorType);
            modelWrapper.SpecificDbContext = GetSpecificDbContext(dbContextType, configuratorType);
            
            InitializeConfigurator(modelWrapper);
        }

        private static Type GetSpecificDbContext(Type dbContextType, Type configuratorType)
        {
            return AllDbContextTypes.FirstOrDefault(type =>
            {
                if (!dbContextType.IsAssignableFrom(type))
                    return false;
                
                var modelConfiguratorAttr = type.GetCustomAttribute<ModelConfiguratorAttribute>();
                if (modelConfiguratorAttr == null)
                    return false;
                
                return modelConfiguratorAttr.ConfiguratorType == configuratorType;
            }) ?? dbContextType;
        }

        private void InitializeConfigurator(ModelWrapper modelWrapper)
        {
            var configuratorType = modelWrapper.Configurator.GetType();
            var logger = _loggerFactory.CreateLogger(configuratorType);
            modelWrapper.Configurator.Initialize(modelWrapper.SpecificDbContext, _configManager, logger);
        }

        private string ConfigFilename(Type dbContextType)
            => dbContextType.FullName + ".DbConfig";

        /// <inheritdoc />
        public IReadOnlyCollection<Type> Contexts => _knownModels.Select(km => km.DbContextType).ToArray();

        /// <inheritdoc />
        public IModelConfigurator GetConfigurator(Type contextType) => _knownModels.First(km => km.DbContextType == contextType).Configurator;

        /// <inheritdoc />
        public IModelSetupExecutor GetSetupExecutor(Type contextType)
        {
            var setupExecutorType = typeof(ModelSetupExecutor<>).MakeGenericType(contextType);
            return (IModelSetupExecutor)Activator.CreateInstance(setupExecutorType, this);
        }

        /// <inheritdoc />
        public TContext Create<TContext>() where TContext : DbContext =>
            Create<TContext>(null);

        /// <inheritdoc />
        public TContext Create<TContext>(IDatabaseConfig config) where TContext : DbContext
        {
            var wrapper = _knownModels.FirstOrDefault(k => k.DbContextType == typeof(TContext));
            if (wrapper == null)
                throw new InvalidOperationException("Unknown model");

            var configurator = wrapper.Configurator;

            return config != null
                ? (TContext)configurator.CreateContext(config)
                : (TContext)configurator.CreateContext();
        }

        private class ModelWrapper
        {
            public Type DbContextType { get; set; }

            public IModelConfigurator Configurator { get; set; }
            
            public Type SpecificDbContext { get; set; }
        }
    }
}