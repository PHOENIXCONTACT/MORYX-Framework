// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Runtime.Modules;
using Moryx.Tools;

namespace Moryx.Runtime.Kernel
{
    internal class ModuleStarter : ModuleManagerComponent, IModuleStarter
    {
        private readonly IModuleDependencyManager _dependencyManager;
        private readonly ILogger _logger;
        private readonly ModuleManagerConfig _config;
        private readonly SemaphoreSlim _waitingModulesSemaphore = new(1, 1);

        public ModuleStarter(IModuleDependencyManager dependencyManager, ILogger logger, ModuleManagerConfig config)
        {
            _dependencyManager = dependencyManager;
            _logger = logger;
            _config = config;
        }

        /// <inheritdoc />
        public Task InitializeAsync(IServerModule module, CancellationToken cancellationToken)
        {
            if (!AvailableModules.Contains(module))
                return Task.CompletedTask; // Module not executable

            return module.InitializeAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task StartAsync(IServerModule module, CancellationToken cancellationToken)
        {
            if (!AvailableModules.Contains(module))
                return; // Module not executable

            await module.InitializeAsync(cancellationToken);

            await StartModule(module, cancellationToken);
        }

        /// <inheritdoc />
        public async Task StartAllAsync(CancellationToken cancellationToken)
        {
            foreach (var module in AvailableModules)
            {
                await module.InitializeAsync(cancellationToken);
            }

            // Find root server modules and convert all others to waiting services
            var depTree = _dependencyManager.GetDependencyTree();
            foreach (var root in depTree.RootModules.Where(ShouldBeStarted))
            {
                ConvertBranch(root);
            }

            foreach (var module in depTree.RootModules.Where(ShouldBeStarted).Select(branch => branch.RepresentedModule))
            {
                await StartModule(module, cancellationToken);
            }
        }

        private async Task StartModule(IServerModule module, CancellationToken cancellationToken)
        {
            // Check for any failed dependencies
            var hasFailedDependencies = _dependencyManager.GetDependencyBranch(module).Dependencies
                                     .Any(item => item.RepresentedModule.State == ServerModuleState.Failure);
            // Don't try to start modules which initialization has been failed or for which dependency initializations have failed
            if (module.State == ServerModuleState.Failure || hasFailedDependencies)
                return;

            // Now we check for any not running dependencies and start them
            var awaitingDependencies = _dependencyManager.GetDependencyBranch(module).Dependencies
                                     .Where(item => !item.RepresentedModule.State.HasFlag(ServerModuleState.Running))
                                     // Filter missing modules if they are optional
                                     .Where(item => item.RepresentedModule is not MissingServerModule { Optional: true })
                                     .Select(item => item.RepresentedModule).ToArray();

            if (awaitingDependencies.Length != 0)
            {
                await EnqueueServiceAndStartDependencies(awaitingDependencies, module, cancellationToken);
            }
            else
            {
                _ = Task.Run(async () => await ExecuteModuleStart(module, cancellationToken), cancellationToken);
            }
        }

        private async Task ExecuteModuleStart(IServerModule module, CancellationToken cancellationToken)
        {
            // Should be caught by ServerModuleBase but better be safe than sorry
            try
            {
                await module.InitializeAsync(cancellationToken);
                await module.StartAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start module {name}", module.Name);
            }

            // Forward result
            await ModuleChangedState(module, module.State, cancellationToken);
        }


        private async Task ModuleChangedState(IServerModule module, ServerModuleState newState, CancellationToken cancellationToken)
        {
            // Check if it switched to running
            if (!newState.HasFlag(ServerModuleState.Running))
                return;

            // Now we start every service waiting on this service to return
            await _waitingModulesSemaphore.ExecuteAsync(async () =>
            {
                if (!WaitingModules.TryGetValue(module, out var value))
                    return;

                // To increase boot speed we fork module start if more than one dependent was found
                foreach (var waitingModule in value.ToArray())
                {
                    value.Remove(waitingModule);
                    await StartModule(waitingModule, cancellationToken);
                }

                // We remove this service for now after we started every dependent
                WaitingModules.Remove(module);
            }, cancellationToken);
        }

        private void ConvertBranch(IModuleDependency branch)
        {
            foreach (var dependent in branch.Dependents.Where(ShouldBeStarted))
            {
                AddWaitingModule(branch.RepresentedModule, dependent.RepresentedModule);
                ConvertBranch(dependent);
            }
        }

        private async Task EnqueueServiceAndStartDependencies(IEnumerable<IServerModule> dependencies, IServerModule waitingService, CancellationToken cancellationToken)
        {
            foreach (var dependency in dependencies)
            {
                AddWaitingModule(dependency, waitingService);
                await StartAsync(dependency, cancellationToken);
            }
        }

        private bool ShouldBeStarted(IModuleDependency plugin)
        {
            var conf = _config.GetOrCreate(plugin.RepresentedModule.Name);
            var result = conf.StartBehaviour == ModuleStartBehaviour.Auto || plugin.Dependents.Any(ShouldBeStarted);
            return result;
        }
    }
}
