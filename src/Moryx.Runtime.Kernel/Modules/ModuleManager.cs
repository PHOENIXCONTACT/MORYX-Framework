// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using Marvin.Configuration;
using Marvin.Container;
using Marvin.Logging;
using Marvin.Modules;
using Marvin.Runtime.Modules;

namespace Marvin.Runtime.Kernel
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
        /// Get/set the list of server modules. Injected by castle.
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
        private IModuleInitializer _moduleInitializer;
        private IModuleStarter _moduleStarter;
        private IModuleStopper _moduleStopper;
        private IModuleFailureStrategy _failureStrategy;
        private ModuleManagerConfig _config;

        private readonly IDictionary<IServerModule, int> _retryCount = new Dictionary<IServerModule, int>();

        #endregion

        #region IModuleManager

        /// <summary>
        /// Initialize the module manager.
        /// </summary>
        public void Initialize()
        {
            // Create cross component objects
            var waitingServices = new Dictionary<IServerModule, ICollection<IServerModule>>();
            var allModules = new Func<IEnumerable<IServerModule>>(() => AllModules);

            // Create components
            LoggerManagement.ActivateLogging(this);
            _config = ConfigManager.GetConfiguration<ModuleManagerConfig>();
            _dependencyManager = new ModuleDependencyManager(Logger.GetChild(string.Empty, typeof(ModuleDependencyManager)));

            _moduleInitializer = new ModuleInitializer(Logger.GetChild(string.Empty, typeof(ModuleInitializer)));
            _moduleStarter = new ModuleStarter(_dependencyManager, Logger.GetChild(string.Empty, typeof(ModuleStarter)), _config);
            _moduleStopper = new ModuleStopper(_dependencyManager, Logger.GetChild(string.Empty, typeof(ModuleStopper)));

            _failureStrategy = new FailureStrategy(_config);

            // Link components
            var components = new IModuleManagerComponent[] { _dependencyManager, _moduleStarter, _moduleStopper };
            foreach (var component in components)
            {
                component.AllModules = allModules;
                component.WaitingModules = waitingServices;
            }

            // Build dependency tree
            _dependencyManager.BuildDependencyTree();

            // Link framework modules
            foreach (var platformModule in ServerModules.OfType<IPlatformModule>())
            {
                platformModule.SetModuleManager(this);
            }

            // Observe state changed events of modules
            foreach (var module in AllModules)
            {
                module.StateChanged += OnModuleStateChanged;
            }
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
            _moduleInitializer.Initialize(module);
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
        /// Restart the module and all of its dependecies
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
        public IEnumerable<IServerModule> AllModules => ServerModules;

        /// <summary>
        /// Inform the listeners about module state changes. 
        /// </summary>
        public event EventHandler<ModuleStateChangedEventArgs> ModuleStateChanged;

        /// <summary>
        /// Get the start dependencies of the given module.
        /// </summary>
        /// <param name="service">The server module for which the dependencies should be fetched.</param>
        /// <returns>An amount of start dependecies for the requested module.</returns>
        public IEnumerable<IServerModule> StartDependencies(IServerModule service)
        {
            return _dependencyManager.GetDependencyBranch(service).Dependencies.Select(item => item.RepresentedModule).ToArray();
        }

        /// <summary>
        /// Get the full dependency tree
        /// </summary>
        /// <returns></returns>
        public IDependencyEvaluation DependencyEvaluation => _dependencyManager.GetDependencyEvalutaion();

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
            ReincarnateOnFailure((IServerModule)sender, eventArgs.NewState);
        }

        private void ReincarnateOnFailure(IServerModule sender, ServerModuleState newState)
        {
            switch (newState)
            {
                case ServerModuleState.Running:
                    // Module started successfully at least once, so it can be restarted
                    if (!_retryCount.ContainsKey(sender))
                        _retryCount[sender] = 0;
                    break;

                case ServerModuleState.Stopping:
                    // After stopping retry is pointless until next successful start
                    if (_retryCount.ContainsKey(sender))
                        _retryCount.Remove(sender);
                    break;

                case ServerModuleState.Failure:
                    // Not started so we can not try to reboot it
                    if (!_retryCount.ContainsKey(sender))
                        break;

                    // Try to restart if configured
                    if (_failureStrategy.ReincarnateOnFailure(sender) &&
                        _retryCount[sender] <= _config.MaxRetries)
                    {
                        _retryCount[sender]++;
                        ReincarnateModule(sender);
                    }
                    break;
            }
        }
    }
}
