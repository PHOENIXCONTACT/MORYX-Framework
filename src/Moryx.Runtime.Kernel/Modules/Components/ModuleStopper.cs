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

        /// <inheritdoc/>
        public async Task StopAsync(IServerModule module, CancellationToken cancellationToken)
        {
            if (!AvailableModules.Contains(module))
                return;

            // First we have to find all running modules that depend on this service
            var dependingServices = _dependencyManager.GetDependencyBranch(module).Dependents.Select(item => item.RepresentedModule);
            // Now we will stop all of them recursively
            foreach (var dependingService in dependingServices.Where(dependent => dependent.State.HasFlag(ServerModuleState.Running)
                                                                               || dependent.State == ServerModuleState.Starting))
            {
                // We will enqueue the service to make sure it is restarted later on
                AddWaitingModule(module, dependingService);
                await StopAsync(dependingService, cancellationToken);
            }

            // Since stop is synchronous we don't need an event
            try
            {
                await module.StopAsync(cancellationToken);
            }
            catch
            {
                _logger.LogError("Failed to stop module <{moduleName}>", module.Name);
            }
        }

        /// <inheritdoc/>
        public async Task StopAllAsync(CancellationToken cancellationToken)
        {
            // Determine all leaves of the dependency tree
            foreach (var module in AvailableModules)
            {
                await StopAsync(module, cancellationToken);
            }
        }
    }
}
