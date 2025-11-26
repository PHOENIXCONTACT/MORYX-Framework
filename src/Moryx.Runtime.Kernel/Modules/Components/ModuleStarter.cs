// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Runtime.Kernel.Modules;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel
{
    internal class ModuleStarter : ModuleManagerComponent, IModuleStarter
    {
        private readonly IModuleDependencyManager _dependencyManager;
        private readonly ILogger _logger;
        private readonly ModuleManagerConfig _config;

        public ModuleStarter(IModuleDependencyManager dependencyManager, ILogger logger, ModuleManagerConfig config)
        {
            _dependencyManager = dependencyManager;
            _logger = logger;
            _config = config;
        }

        /// <inheritdoc />
        public Task InitializeAsync(IServerModule module)
        {
            if (!AvailableModules.Contains(module))
                return Task.CompletedTask; // Module not executable

            return module.Initialize();
        }

        /// <inheritdoc />
        public async Task StartAsync(IServerModule module)
        {
            if (!AvailableModules.Contains(module))
                return; // Module not executable

            await module.Initialize();

            await StartModule(module);
        }

        /// <inheritdoc />
        public async Task StartAllAsync()
        {
            foreach (var module in AvailableModules)
            {
                await module.Initialize();
            }

            // Find root server modules and convert all others to waiting services
            var depTree = _dependencyManager.GetDependencyTree();
            foreach (var root in depTree.RootModules.Where(ShouldBeStarted))
            {
                ConvertBranch(root);
            }
            foreach (var module in depTree.RootModules.Where(ShouldBeStarted).Select(branch => branch.RepresentedModule))
            {
                await StartModule(module);
            }
        }

        private async Task StartModule(IServerModule module)
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
                                     .Where(item => item.RepresentedModule is not MissingServerModule module || !module.Optional)
                                     .Select(item => item.RepresentedModule).ToArray();
            if (awaitingDependencies.Length != 0)
                await EnqueServiceAndStartDependencies(awaitingDependencies, module);
            else
                Task.Run(() => ExecuteModuleStart(module));
        }

        private async Task ExecuteModuleStart(IServerModule module)
        {
            // Should be caught by ServerModuleBase but better be safe than sorry 
            try
            {
                await module.Initialize();
                await module.StartAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start module {name}", module.Name);
            }
            // Forward result
            await ModuleChangedState(module, module.State);
        }

        private Task ModuleChangedState(IServerModule module, ServerModuleState newState)
        {
            // Check if it switched to running
            if (!newState.HasFlag(ServerModuleState.Running))
                return Task.CompletedTask;

            // Now we start every service waiting on this service to return
            lock (WaitingModules)
            {
                if (!WaitingModules.ContainsKey(module))
                    return Task.CompletedTask;

                // To increase boot speed we fork plugin start if more than one dependent was found
                foreach (var waitingModule in WaitingModules[module].ToArray())
                {
                    WaitingModules[module].Remove(waitingModule);
                    StartModule(waitingModule);
                }
                // We remove this service for now after we started every dependent
                WaitingModules.Remove(module);
            }

            return Task.CompletedTask;
        }

        private void ConvertBranch(IModuleDependency branch)
        {
            foreach (var dependent in branch.Dependents.Where(ShouldBeStarted))
            {
                AddWaitingService(branch.RepresentedModule, dependent.RepresentedModule);
                ConvertBranch(dependent);
            }
        }

        private async Task EnqueServiceAndStartDependencies(IEnumerable<IServerModule> dependencies, IServerModule waitingService)
        {
            foreach (var dependency in dependencies)
            {
                AddWaitingService(dependency, waitingService);
                await StartAsync(dependency);
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
