// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
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

        public DbContextManager(IConfigManager configManager, ILoggerFactory loggerFactory)
        {
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
                        Configurator = (IModelConfigurator)Activator.CreateInstance(t.ModelConfiguratorAttr.ConfiguratorType)
                    };
                    return wrapper;
                }).ToArray();

            foreach (var wrapper in _knownModels)
            {
                var configuratorType = wrapper.Configurator.GetType();
                var logger =loggerFactory.CreateLogger(configuratorType);
                wrapper.Configurator.Initialize(wrapper.DbContextType, configManager, logger);
            }
        }

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