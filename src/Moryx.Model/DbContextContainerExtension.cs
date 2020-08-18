// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Model.Repositories;

namespace Moryx.Model
{
    /// <summary>
    /// Extension to activate database access in the local container
    /// </summary>
    public static class DbContextContainerExtension
    {
        /// <summary>
        /// Register <see cref="IDbContextManager"/> and <see cref="IContextFactory{TContext}"/>
        /// </summary>
        public static IContainer ActivateDbContexts(this IContainer container, IDbContextManager contextManager)
        {
            container.SetInstance(contextManager);
            container.ExecuteInstaller(new ContextFactoryInstaller());

            return container;
        }

        private class ContextFactoryInstaller : IContainerInstaller
        {
            public void Install(IComponentRegistrator registrator)
            {
                registrator.Register(typeof(ContextFactory<>), new []{ typeof(IContextFactory<>) }, "GenericContextFactory", LifeCycle.Singleton);
                registrator.Register(typeof(UnitOfWorkFactory<>), new[] { typeof(IUnitOfWorkFactory<>) }, "UnitOfWorkFactory", LifeCycle.Singleton);
            }
        }
    }
}