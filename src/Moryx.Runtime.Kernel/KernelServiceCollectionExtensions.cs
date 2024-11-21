// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.DependencyInjection;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Runtime.Modules;
using Moryx.Threading;
using System;
using System.IO;
using System.Linq;

namespace Moryx.Runtime.Kernel
{
    /// <summary>
    /// Contains <seealso cref="IServiceCollection"/> extensions to easily add MORYX to an Asp.Net Core application
    /// </summary>
    public static class KernelServiceCollectionExtensions
    {
        /// <summary>
        /// Link MORYX kernel to the service collection
        /// </summary>
        public static void AddMoryxKernel(this IServiceCollection serviceCollection)
        {
            // Register config manager
            serviceCollection.AddSingleton<ConfigManager>();
            serviceCollection.AddSingleton<IConfigManager>(x => x.GetRequiredService<ConfigManager>());

            // Register module manager
            serviceCollection.AddSingleton<ModuleManager>();
            serviceCollection.AddSingleton<IModuleManager>(x => x.GetRequiredService<ModuleManager>());

            // Register parallel operations
            serviceCollection.AddTransient<IParallelOperations, ParallelOperations>();

            // Register container factory for module container
            serviceCollection.AddSingleton<IModuleContainerFactory, ModuleContainerFactory>();

            // Initialize Platform class
            Platform.SetPlatform();
        }

        /// <summary>
        /// Add MORYX modules to the service collection
        /// </summary>
        public static void AddMoryxModules(this IServiceCollection serviceCollection)
        {           
            // Find all module types in the app domain
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(p => !p.IsDynamic);
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
            var configManager = serviceProvider.GetRequiredService<ConfigManager>(); 
            if (!Directory.Exists(configDirectory))
                Directory.CreateDirectory(configDirectory);
            configManager.ConfigDirectory = configDirectory;
            return configManager;
        }

        private static IModuleManager _moduleManager;
        /// <summary>
        /// Boot system and start all modules
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        [Obsolete("Resolve IModuleManager and call StartModules directly")]
        public static IModuleManager StartMoryxModules(this IServiceProvider serviceProvider)
        {
            var moduleManager = serviceProvider.GetRequiredService<IModuleManager>();
            moduleManager.StartModules();
            return _moduleManager = moduleManager;
        }

        /// <summary>
        /// Stop all modules
        /// </summary>
        [Obsolete("Stopping modules on service collection causes an exception, call StopModules on the return value of StartModules")]
        public static IModuleManager StopMoryxModules(this IServiceProvider serviceProvider)
        {
            var moduleManager = _moduleManager ?? serviceProvider.GetRequiredService<IModuleManager>();
            moduleManager.StopModules();
            return moduleManager;
        }
    }
}
