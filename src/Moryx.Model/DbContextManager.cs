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
    public class DbContextManager : IDbContextManager
    {
        private ModelWrapper[] _knownModels;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfigManager _configManager;

        /// <inheritdoc />
        public DbContextManager(IConfigManager configManager, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _configManager = configManager;

            var dbContextTypes = ReflectionTool.GetPublicClasses(
                typeof(DbContext), 
                type => type != typeof(DbContext) && !type.GetCustomAttributes<DatabaseSpecificContextAttribute>().Any());

            _knownModels = dbContextTypes
                .Select(dbContextType =>
                {
                    var config = configManager.GetConfiguration<DatabaseConfig<DatabaseConnectionSettings>>(ConfigFilename(dbContextType));

                    var configuratorType = !string.IsNullOrEmpty(config.ConfiguratorTypename)
                        ? Type.GetType(config.ConfiguratorTypename)
                        : typeof(NullModelConfigurator);

                    return new
                    {
                        DbContextType = dbContextType,
                        ConfiguratorType = configuratorType,
                    };
                }).Select(t =>
                {
                    var wrapper = new ModelWrapper
                    {
                        DbContextType = t.DbContextType,
                        Configurator = (IModelConfigurator)Activator.CreateInstance(t.ConfiguratorType)
                    };
                    return wrapper;
                }).ToArray();

            foreach (var wrapper in _knownModels)
            {
                InitializeConfigurator(wrapper);
            }
        }

        /// <inheritdoc />
        public void UpdateConfig(Type dbContextType, Type configuratorType, IDatabaseConfig databaseConfig)
        {
            _configManager.SaveConfiguration(databaseConfig, ConfigFilename(dbContextType));

            var modelWrapper = _knownModels.First(w => w.DbContextType == dbContextType);
            modelWrapper.Configurator = (IModelConfigurator)Activator.CreateInstance(configuratorType);

            InitializeConfigurator(modelWrapper);
        }

        private void InitializeConfigurator(ModelWrapper modelWrapper)
        {
            var configuratorType = modelWrapper.Configurator.GetType();
            var logger = _loggerFactory.CreateLogger(configuratorType);
            modelWrapper.Configurator.Initialize(modelWrapper.DbContextType, _configManager, logger);
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
        }
    }
}