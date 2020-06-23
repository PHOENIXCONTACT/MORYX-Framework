// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Moryx.Configuration;
using Moryx.Model;
using Moryx.Modules;
using Moryx.Runtime.Maintenance.Plugins.Databases;
using DatabaseConfig = Moryx.Model.DatabaseConfig;

namespace Moryx.TestTools.SystemTest
{
    /// <summary>
    /// Creation context for the parent of a merged datamodel
    /// </summary>
    /// <typeparam name="TFactory">Concrete IUnitOfWorkFactory type</typeparam>
    public class FactoryCreationContext<TFactory>
        where TFactory : class, IUnitOfWorkFactory, new()
    {
        private readonly IWindsorContainer _container;

        /// <summary>
        /// Create a new creation context instance
        /// </summary>
        public FactoryCreationContext()
        {
            _container = new WindsorContainer();
            _container.Register(Component.For<IConfigManager, ConfigManagerMock>().ImplementedBy<ConfigManagerMock>());
        }

        private void RegisterConfig(DatabaseConfigModel config)
        {
            var dbconfig = new DatabaseConfig
            {
                Host = config.Server,
                ConfigState = ConfigState.Valid,
                Database = config.Database,
                Username = config.User,
                Password = config.Password,
            };
            _container.Resolve<ConfigManagerMock>().SaveConfiguration(dbconfig);;
        }

        /// <summary>
        /// Build the factory for a given configuration
        /// </summary>
        /// <typeparam name="TConfig">Concrete IDatabaseConfig type</typeparam>
        /// <param name="databaseConfigModel">Config model containing the connections settings</param>
        public void SetConfig(DatabaseConfigModel databaseConfigModel)
        {
            // Create a database config and register at config manager
            RegisterConfig(databaseConfigModel);

            // Register factory for later
            _container.Register(Component.For<TFactory>());
            ((IInitializable)_container.Resolve<TFactory>()).Initialize();
        }

        /// <summary>
        /// Build factory instance
        /// </summary>
        public T Build<T>()
        {
            return _container.Resolve<T>();
        }

        /// <summary>
        /// Merge this factory with a given child
        /// </summary>
        /// <typeparam name="TChildFactory"></typeparam>
        /// <param name="config"></param>
        /// <param name="mergeOperation"></param>
        /// <returns></returns>
        public FactoryCreationContext<TFactory> Merge<TChildFactory>(DatabaseConfigModel config, Action<TFactory, TChildFactory> mergeOperation)
            where TChildFactory : class, IUnitOfWorkFactory, new()
        {
            // Register child config
            RegisterConfig(config);

            // Register child factory
            _container.Register(Component.For<TChildFactory>());

            // Resolve and merge
            var childFac = _container.Resolve<TChildFactory>();
            mergeOperation(_container.Resolve<TFactory>(), childFac);

            // Initialize child
            ((IInitializable)childFac).Initialize();

            return this;
        }
    }
}
