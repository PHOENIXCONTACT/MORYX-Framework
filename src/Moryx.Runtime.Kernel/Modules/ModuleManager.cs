// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel
{
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

        /// <summary>
        /// Start all modules in cascading order
        /// </summary>
        public Task StartModulesAsync()
        {
            return _moduleStarter.StartAllAsync();
        }

        /// <summary>
        /// Stop all modules in cascading order
        /// </summary>
        public Task StopModulesAsync()
        {
            return _moduleStopper.StopAllAsync();
        }

        /// <summary>
        /// Initialize a server module
        /// </summary>
        /// <param name="module"></param>
        public Task InitializeModule(IServerModule module)
        {
            return _moduleStarter.InitializeAsync(module);
        }

        /// <summary>
        /// Start a specific module and all its dependencies
        /// </summary>
        /// <param name="module">Module to start</param>
        public Task StartModule(IServerModule module)
        {
            return _moduleStarter.StartAsync(module);
        }

        /// <summary>
        /// Stop a specific module
        /// </summary>
        /// <param name="module">Module to stop</param>
        public Task StopModule(IServerModule module)
        {
            return _moduleStopper.StopAsync(module);
        }

        /// <summary>
        /// Restart the module and all of its dependencies
        /// </summary>
        /// <param name="module"></param>
        public async Task ReincarnateModule(IServerModule module)
        {
            // Stop execution
            await _moduleStopper.StopAsync(module);

            // Start all desired
            await _moduleStarter.StartAsync(module);
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
}
