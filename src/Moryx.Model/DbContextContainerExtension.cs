// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.DependencyInjection;
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
        public static IServiceCollection ActivateDbContexts(this IServiceCollection services, IDbContextManager contextManager)
        {
            services.AddSingleton(contextManager);

            services.AddSingleton(typeof(IContextFactory<>), typeof(ContextFactory<>));
            services.AddSingleton(typeof(IUnitOfWorkFactory<>), typeof(UnitOfWorkFactory<>));

            return services;
        }
    }
}