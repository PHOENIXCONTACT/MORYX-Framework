// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
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

        /// <inheritdoc />
        public string Name => "Kernel";

        private IModuleDependencyManager _dependencyManager;

        private IModuleStarter _moduleStarter;
        private IModuleStopper _moduleStopper;
        private ModuleManagerConfig _config;

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
        public void StartModules()
        {
            _moduleStarter.StartAll();
        }

        /// <summary>
        /// Stop all modules in cascading order
        /// </summary>
        public void StopModules()
        {
            _moduleStopper.StopAll();
        }

        /// <summary>
        /// Initialize a server module
        /// </summary>
        /// <param name="module"></param>
        public void InitializeModule(IServerModule module)
        {
            _moduleStarter.Initialize(module);
        }

        /// <summary>
        /// Start a specific module and all its dependencies
        /// </summary>
        /// <param name="module">Module to start</param>
        public void StartModule(IServerModule module)
        {
            _moduleStarter.Start(module);
        }

        /// <summary>
        /// Stop a specific module
        /// </summary>
        /// <param name="module">Module to stop</param>
        public void StopModule(IServerModule module)
        {
            _moduleStopper.Stop(module);
        }

        /// <summary>
        /// Restart the module and all of its dependencies
        /// </summary>
        /// <param name="module"></param>
        public void ReincarnateModule(IServerModule module)
        {
            // Stop execution
            _moduleStopper.Stop(module);

            // Start all desired
            _moduleStarter.Start(module);
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
