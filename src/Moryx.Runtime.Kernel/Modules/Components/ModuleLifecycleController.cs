// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Runtime.Modules;
using Moryx.Tools;

namespace Moryx.Runtime.Kernel;

internal class ModuleLifecycleController
{
    private readonly IReadOnlyList<IServerModule> _availableModules;
    private readonly IModuleDependencyManager _dependencyManager;
    private readonly ILogger _logger;
    private readonly ModuleManagerConfig _config;
    private readonly SemaphoreSlim _waitingModulesSemaphore = new(1, 1);
    private readonly Dictionary<IServerModule, ICollection<IServerModule>> _waitingModules = new();

    public ModuleLifecycleController(IReadOnlyList<IServerModule> availableModules,
        IModuleDependencyManager dependencyManager, ILogger logger, ModuleManagerConfig config)
    {
        _availableModules = availableModules;
        _dependencyManager = dependencyManager;
        _logger = logger;
        _config = config;
    }

    public Task InitializeAsync(IServerModule module, CancellationToken cancellationToken)
    {
        if (!_availableModules.Contains(module))
            return Task.CompletedTask;

        return module.InitializeAsync(cancellationToken);
    }

    public async Task StartAsync(IServerModule module, CancellationToken cancellationToken)
    {
        if (!_availableModules.Contains(module))
            return;

        await module.InitializeAsync(cancellationToken);
        await StartModule(module, cancellationToken);
    }

    public async Task StartAllAsync(CancellationToken cancellationToken)
    {
        foreach (var module in _availableModules)
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

    public async Task StopAsync(IServerModule module, CancellationToken cancellationToken)
    {
        if (!_availableModules.Contains(module))
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

        // State machine handles error transitions internally
        await module.StopAsync(cancellationToken);
    }

    public async Task StopAllAsync(CancellationToken cancellationToken)
    {
        foreach (var module in _availableModules)
        {
            await StopAsync(module, cancellationToken);
        }
    }

    private async Task StartModule(IServerModule module, CancellationToken cancellationToken)
    {
        var dependencies = _dependencyManager.GetDependencyBranch(module).Dependencies;

        // Check for any failed dependencies
        var hasFailedDependencies = dependencies
            .Any(item => item.RepresentedModule.State == ServerModuleState.Failure);

        // Don't try to start modules which initialization has been failed or for which dependency initializations have failed
        if (module.State == ServerModuleState.Failure || hasFailedDependencies)
            return;

        // Now we check for any not running dependencies and start them
        var awaitingDependencies = dependencies
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
        // State machine owns initialization and start transitions including error handling
        await module.StartAsync(cancellationToken);

        // Forward result to start waiting dependents
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
            if (!_waitingModules.TryGetValue(module, out var previouslyWaitingModules))
                return;

            // To increase boot speed we fork module start if more than one dependent was found
            foreach (var waitingModule in previouslyWaitingModules.ToArray())
            {
                previouslyWaitingModules.Remove(waitingModule);
                await StartModule(waitingModule, cancellationToken);
            }

            // We remove this service for now after we started every dependent
            _waitingModules.Remove(module);
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

    private void AddWaitingModule(IServerModule dependency, IServerModule dependent)
    {
        lock (_waitingModules)
        {
            if (_waitingModules.TryGetValue(dependency, out var waitingModules))
            {
                if (!waitingModules.Contains(dependent))
                    waitingModules.Add(dependent);
            }
            else
            {
                _waitingModules[dependency] = new List<IServerModule> { dependent };
            }
        }
    }
}
