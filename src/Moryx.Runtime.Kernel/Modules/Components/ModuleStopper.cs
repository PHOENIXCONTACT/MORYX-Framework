// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel
{
    internal class ModuleStopper : ModuleManagerComponent, IModuleStopper
    {
        private readonly IModuleDependencyManager _dependencyManager;
        private readonly ILogger _logger;

        public ModuleStopper(IModuleDependencyManager dependencyManager, ILogger logger)
        {
            _dependencyManager = dependencyManager;
            _logger = logger;
        }

        /// <summary>
        /// Stop this plugin and all required dependencies
        /// </summary>
        /// <param name="module"></param>
        public async Task StopAsync(IServerModule module)
        {
            if (!AvailableModules.Contains(module))
                return;

            // First we have to find all running modules that depend on this service
            var dependingServices = _dependencyManager.GetDependencyBranch(module).Dependents.Select(item => item.RepresentedModule);
            // Now we will stop all of them recursivly
            foreach (var dependingService in dependingServices.Where(dependent => dependent.State.HasFlag(ServerModuleState.Running)
                                                                               || dependent.State == ServerModuleState.Starting))
            {
                // We will enque the service to make sure it is restarted later on
                AddWaitingService(module, dependingService);
                await StopAsync(dependingService);
            }

            // Since stop is synchron we don't need an event
            try
            {
                await module.StopAsync();
            }
            catch
            {
                Console.WriteLine("Failed to stop service <{0}>", module.Name);
            }
        }

        /// <summary>
        /// Stop all services
        /// </summary>
        public async Task StopAllAsync()
        {
            // Determine all leaves of the dependency tree
            foreach (var plugin in AvailableModules)
            {
                await StopAsync(plugin);
            }
        }
    }
}
