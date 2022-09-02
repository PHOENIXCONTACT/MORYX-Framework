// Copyright (c) 2022, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.DependencyInjection;
using Moryx.Asp.Extensions;
using Moryx.Logging;
using Moryx.Model;
using Moryx.Runtime;
using Moryx.Runtime.Configuration;
using Moryx.Runtime.Kernel.Logging;
using Moryx.Runtime.Logging;
using Moryx.Runtime.Modules;
using Moryx.Threading;

namespace Moryx.Asp.Integration
{
    public static class MoryxServiceCollectionExtensions
    {
        /// <summary>
        /// Register MORYX facades in the service collection for module endpoints
        /// </summary>
        public static void AddMoryxFacades(this IServiceCollection serviceCollection, IApplicationRuntime runtime)
        {
            var facadeCollector = runtime.GlobalContainer.Resolve<IFacadeCollector>();
            var facades = facadeCollector.Facades;
            foreach (var facade in facades)
            {
                // Register facade for all its interfaces
                foreach (var facadeApi in facade.GetType().GetInterfaces())
                {
                    serviceCollection.AddSingleton(facadeApi, facade);
                }
            }
        }

        /// <summary>
        /// Link MORYX kernel to the service collection
        /// </summary>
        public static void AddMoryxKernel(this IServiceCollection serviceCollection, IApplicationRuntime runtime)
        {
            // Fetch main kernel components
            var configManager = runtime.GlobalContainer.Resolve<IRuntimeConfigManager>();
            var moduleManager = runtime.GlobalContainer.Resolve<IModuleManager>();
            var loggerManagement = runtime.GlobalContainer.Resolve<IServerLoggerManagement>();
            var dbManager = runtime.GlobalContainer.Resolve<IDbContextManager>();

        // Register in service collection
            serviceCollection.AddSingleton(configManager);
            serviceCollection.AddSingleton(moduleManager);
            serviceCollection.AddSingleton(loggerManagement);
            serviceCollection.AddSingleton<ILoggingAppender, LoggingAppender>();
            serviceCollection.AddSingleton(dbManager);

            serviceCollection.AddTransient<IParallelOperations>((_) =>
            {
                var parallelOps = new ParallelOperations();
                parallelOps.Logger = new LoggerExtension();
                return parallelOps;
            });

            // Register all modules
            foreach (var module in runtime.GlobalContainer.ResolveAll<IServerModule>())
            {
                serviceCollection.AddSingleton(module);
            }
        }
    }
}
