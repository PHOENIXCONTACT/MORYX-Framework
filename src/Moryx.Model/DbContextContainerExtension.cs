// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
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

            container.Register(typeof(ContextFactory<>), [typeof(IContextFactory<>)], "GenericContextFactory", LifeCycle.Singleton);
            container.Register(typeof(UnitOfWorkFactory<>), [typeof(IUnitOfWorkFactory<>)], "UnitOfWorkFactory", LifeCycle.Singleton);

            return container;
        }
    }
}