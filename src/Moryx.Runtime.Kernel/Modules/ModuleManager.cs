// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel;

/// <summary>
/// Manages all the modules on the server side.
/// </summary>
public class ModuleManager : IModuleManager
{
    #region Fields and Properties

    private readonly IModuleDependencyManager _dependencyManager;

    private readonly IModuleStarter _moduleStarter;
    private readonly IModuleStopper _moduleStopper;
    private readonly ModuleManagerConfig _config;

    #endregion

    #region IModuleManager

    /// <summary>
    ///
    /// </summary>
    public ModuleManager(IEnumerable<IServerModule> modules, IConfigManager configManager, ILogger<ModuleManager> logger)
    {
        var allModules = modules.ToList();

        // Create components
        _config = configManager.GetConfiguration<ModuleManagerConfig>();

        // Create dependency manager and build tree of available modules
        _dependencyManager = new ModuleDependencyManager(logger);
        var availableModules = _dependencyManager.BuildDependencyTree(allModules);

        // Create dedicated components for stopping and starting
        var waitingModules = new Dictionary<IServerModule, ICollection<IServerModule>>();
        _moduleStarter = new ModuleStarter(_dependencyManager, logger, _config)
        {
            AvailableModules = availableModules,
            WaitingModules = waitingModules
        };
        _moduleStopper = new ModuleStopper(_dependencyManager, logger)
        {
            AvailableModules = availableModules,
            WaitingModules = waitingModules
        };

        // Observe state changed events of modules
        foreach (var module in availableModules)
        {
            module.StateChanged += OnModuleStateChanged;
        }

        AllModules = allModules;
    }

    /// <inheritdoc/>
    public Task StartModulesAsync(CancellationToken cancellationToken = default)
    {
        return _moduleStarter.StartAllAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public Task StopModulesAsync(CancellationToken cancellationToken = default)
    {
        return _moduleStopper.StopAllAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public Task InitializeModuleAsync(IServerModule module, CancellationToken cancellationToken = default)
    {
        return _moduleStarter.InitializeAsync(module, cancellationToken);
    }

    /// <inheritdoc/>
    public Task StartModuleAsync(IServerModule module, CancellationToken cancellationToken = default)
    {
        return _moduleStarter.StartAsync(module, cancellationToken);
    }

    /// <inheritdoc/>
    public Task StopModuleAsync(IServerModule module, CancellationToken cancellationToken = default)
    {
        return _moduleStopper.StopAsync(module, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task ReincarnateModuleAsync(IServerModule module, CancellationToken cancellationToken = default)
    {
        // Stop execution
        await _moduleStopper.StopAsync(module, cancellationToken);

        // Start all desired
        await _moduleStarter.StartAsync(module, cancellationToken);
    }

    /// <summary>
    /// All modules
    /// </summary>
    public IEnumerable<IServerModule> AllModules { get; private set; }

    /// <summary>
    /// Inform the listeners about module state changes.
    /// </summary>
    public event EventHandler<ModuleStateChangedEventArgs> ModuleStateChanged;

    /// <summary>
    /// Get the start dependencies of the given module.
    /// </summary>
    /// <param name="service">The server module for which the dependencies should be fetched.</param>
    /// <returns>An amount of start dependencies for the requested module.</returns>
    public IEnumerable<IServerModule> StartDependencies(IServerModule service)
    {
        return _dependencyManager.GetDependencyBranch(service)?.Dependencies.Select(item => item.RepresentedModule)
               ?? [];
    }

    /// <summary>
    /// Get the full dependency tree
    /// </summary>
    /// <returns></returns>
    public IModuleDependencyTree DependencyTree => _dependencyManager.GetDependencyTree();

    /// <summary>
    /// Get or set a services behaviour using
    /// </summary>
    /// <typeparam name="T">Type of behaviour</typeparam>
    public IBehaviourAccess<T> BehaviourAccess<T>(IServerModule plugin)
    {
        return ABehaviourAccess.Create<T>(_config, plugin);
    }

    #endregion

    private void OnModuleStateChanged(object sender, ModuleStateChangedEventArgs eventArgs)
    {
        ModuleStateChanged?.Invoke(sender, eventArgs);
    }
}