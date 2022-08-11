// Copyright (c) 2022, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Modules;
using Moryx.Runtime.Configuration;
using Moryx.Runtime.Kernel.Logging;
using Moryx.Runtime.Modules;
using Moryx.Tools;
using System;
using System.Linq;
using System.Reflection;

namespace Moryx.Runtime.Kernel
{
    public static class KernelServiceCollectionExtensions
    {
        /// <summary>
        /// Link MORYX kernel to the service collection
        /// </summary>
        public static void AddMoryxKernel(this IServiceCollection serviceCollection)
        {
            // Register config manager
            serviceCollection.AddSingleton<IRuntimeConfigManager, RuntimeConfigManager>();
            serviceCollection.AddSingleton<IConfigManager>(x => x.GetRequiredService<IRuntimeConfigManager>());

            // Register logging
            serviceCollection.AddSingleton<IModuleLoggerFactory, ModuleLoggerFactory>();

            // Register module manager
            serviceCollection.AddSingleton<ModuleManager>();
            serviceCollection.AddSingleton<IModuleManager>(x => x.GetRequiredService<ModuleManager>());

            // Register container factory for module container
            serviceCollection.AddSingleton<IModuleContainerFactory, ModuleContainerFactory>();
        }

        /// <summary>
        /// Add MORYX modules to the service collection
        /// </summary>
        public static void AddMoryxModules(this IServiceCollection serviceCollection)
        {           
            // Find all module types in the app domain
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var modules = loadedAssemblies
                .SelectMany(assembly => assembly.GetExportedTypes())
                .Where(type => !type.IsInterface & !type.IsAbstract & typeof(IServerModule).IsAssignableFrom(type))
                .ToList();
            foreach(var module in modules)
            {
                // Register module as server module
                serviceCollection.AddSingleton(module);
                serviceCollection.AddSingleton(x => (IServerModule)x.GetRequiredService(module));

                // Determine the exported facades
                var facadeContainers = module.GetInterfaces().Where(api => api.IsGenericType && api.GetGenericTypeDefinition() == typeof(IFacadeContainer<>));
                foreach(var facadeContainer in facadeContainers)
                {
                    // Register module as facade container
                    serviceCollection.AddSingleton(facadeContainer, x => x.GetRequiredService(module));
                    // Forward facade registration to facade container
                    var facade = facadeContainer.GetGenericArguments()[0];
                    serviceCollection.AddSingleton(facade, x =>
                    {
                        var instance = x.GetRequiredService(module);
                        return facadeContainer.GetProperty(nameof(IFacadeContainer<object>.Facade)).GetValue(instance);
                    });
                }
            }
        }

        /// <summary>
        /// Use the moryx config manager
        /// </summary>
        public static IConfigManager UseMoryxConfigurations(this IServiceProvider serviceProvider, string configDirectory)
        {
            // Load config manager
            var configManager = serviceProvider.GetRequiredService<IRuntimeConfigManager>();
            configManager.ConfigDirectory = configDirectory;
            return configManager;
        }

        public static IModuleManager StartMoryxModules(this IServiceProvider serviceProvider)
        {
            var moduleManager = serviceProvider.GetRequiredService<IModuleManager>();
            moduleManager.StartModules();
            return moduleManager;
        }
    }
}
