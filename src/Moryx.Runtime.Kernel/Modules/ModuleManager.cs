// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Modules;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel
{
    /// <summary>
    /// Manages all the modules on the server side. 
    /// </summary>
    [InitializableKernelComponent(typeof(IModuleManager))]
    public class ModuleManager : IModuleManager, IInitializable, ILoggingHost
    {
        #region Dependencies

        /// <summary>
        /// Logger instance
        /// </summary>
        public IModuleLogger Logger { get; set; }

        /// <summary>
        /// Get/set the list of all server modules. Injected by castle.
        /// </summary>
        public IServerModule[] ServerModules { get; set; }

        /// <summary>
        /// Logger management instance. Injected by castle.
        /// </summary>
        public ILoggerManagement LoggerManagement { get; set; }

        /// <summary>
        /// Configuration manager instance. Injected by castel.
        /// </summary>
        public IConfigManager ConfigManager { get; set; }

        #endregion

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
        /// Initialize the module manager.
        /// </summary>
        public void Initialize()
        {
            // Create components
            LoggerManagement.ActivateLogging(this);
            _config = ConfigManager.GetConfiguration<ModuleManagerConfig>();

            // Create dependency manager and build tree of available modules
            _dependencyManager = new ModuleDependencyManager(Logger.GetChild(string.Empty, typeof(ModuleDependencyManager)));
            var availableModules = _dependencyManager.BuildDependencyTree(ServerModules);

            // Create dedicated components for stopping and starting
            var waitingModules = new Dictionary<IServerModule, ICollection<IServerModule>>();
            _moduleStarter = new ModuleStarter(_dependencyManager, Logger.GetChild(string.Empty, typeof(ModuleStarter)), _config)
            {
                AvailableModules = availableModules,
                WaitingModules = waitingModules
            };
            _moduleStopper = new ModuleStopper(_dependencyManager, Logger.GetChild(string.Empty, typeof(ModuleStopper)))
            {
                AvailableModules = availableModules,
                WaitingModules = waitingModules
            };

            // Link framework modules
            foreach (var platformModule in availableModules.OfType<IPlatformModule>())
            {
                platformModule.SetModuleManager(this);
            }

            // Observe state changed events of modules
            foreach (var module in availableModules)
            {
                module.StateChanged += OnModuleStateChanged;
            }

            AllModules = ServerModules;
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
                   ?? Enumerable.Empty<IServerModule>();
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

        #region IFacadeCollector

        /// <inheritdoc />
        public IReadOnlyList<object> Facades => _dependencyManager.Facades;

        #endregion

        private void OnModuleStateChanged(object sender, ModuleStateChangedEventArgs eventArgs)
        {
            ModuleStateChanged?.Invoke(sender, eventArgs);
        }
    }
}
