// Copyright (c) 2022, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.DependencyInjection;
using Moryx.Configuration;
using Moryx.Modules;
using Moryx.Runtime.Configuration;
using Moryx.Runtime.Modules;
using System;
using System.Linq;

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

            // Register module manager
            serviceCollection.AddSingleton<ModuleManager>();
            serviceCollection.AddSingleton<IModuleManager>(x => x.GetRequiredService<ModuleManager>());
            serviceCollection.AddSingleton<IInitializable>(x => x.GetRequiredService<ModuleManager>());    
        }

        /// <summary>
        /// Add MORYX modules to the service collection
        /// </summary>
        public static void AddMoryxModules(this IServiceCollection serviceCollection)
        {
            // Find all module types in the app domain
            var modules = AppDomain.CurrentDomain.GetAssemblies()
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
    }
}
