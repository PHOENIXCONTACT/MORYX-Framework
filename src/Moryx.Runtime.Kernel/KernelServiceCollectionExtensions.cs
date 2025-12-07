// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Runtime.Modules;
using Moryx.Threading;
using Moryx.Tools;

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
            var modules = ReflectionTool.GetPublicClasses<IServerModule>();
            foreach (var module in modules)
            {
                // Register module as server module
                serviceCollection.AddSingleton(module);
                serviceCollection.AddSingleton(x => (IServerModule)x.GetRequiredService(module));

                // Determine the exported facades
                var facadeContainers = module.GetInterfaces().Where(api => api.IsGenericType && api.GetGenericTypeDefinition() == typeof(IFacadeContainer<>));
                foreach (var facadeContainer in facadeContainers)
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
        /// Register BackgroundService that controls that starts and stops the MORYX modules inside the lifetime of the host.
        /// The MORYX Service can hook into the command line and gracefuly stop the modules when the user tries to close the window.
        /// <see cref="Moryx.Runtime.Kernel.MoryxHost"/>
        /// </summary>
        public static void AddMoryxService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddHostedService<MoryxHost>();
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

            // NEW: hook up IConfiguration from the host
            var configuration = serviceProvider.GetService<IConfiguration>();
            if (configuration != null)
                configManager.Configuration = configuration;

            return configManager;
        }
    }
}
