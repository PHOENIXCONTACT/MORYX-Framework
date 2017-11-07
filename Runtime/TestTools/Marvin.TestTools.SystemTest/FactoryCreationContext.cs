using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Marvin.Configuration;
using Marvin.Model;
using Marvin.Modules;
using Marvin.TestTools.SystemTest.DatabaseMaintenance;
using Marvin.TestTools.SystemTest.Mocks;

namespace Marvin.TestTools.SystemTest
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

        private void RegisterConfig<TConfig>(DatabaseConfigModel config)
            where TConfig : IDatabaseConfig, new()
        {
            var dbconfig = new TConfig
            {
                Server = config.Server,
                ConfigState = ConfigState.Valid,
                Database = config.Database,
                User = config.User,
                Password = config.Password,
            };
            _container.Resolve<ConfigManagerMock>().AvailableConfigs.Add(dbconfig);
        }

        /// <summary>
        /// Build the factory for a given configuration
        /// </summary>
        /// <typeparam name="TConfig">Concrete IDatabaseConfig type</typeparam>
        /// <param name="databaseConfigModel">Config model containing the connections settings</param>
        public void SetConfig<TConfig>(DatabaseConfigModel databaseConfigModel)
            where TConfig : IDatabaseConfig, new()
        {
            // Create a database config and register at config manager
            RegisterConfig<TConfig>(databaseConfigModel);

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
        /// <typeparam name="TChildConfig"></typeparam>
        /// <param name="config"></param>
        /// <param name="mergeOperation"></param>
        /// <returns></returns>
        public FactoryCreationContext<TFactory> Merge<TChildFactory, TChildConfig>(DatabaseConfigModel config, Action<TFactory, TChildFactory> mergeOperation)
            where TChildFactory : class, IUnitOfWorkFactory, new()
            where TChildConfig : IDatabaseConfig, new()
        {
            // Register child config
            RegisterConfig<TChildConfig>(config);

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