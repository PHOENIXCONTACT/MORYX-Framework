using System;
using System.Collections.Generic;
using System.Linq;
using Marvin.Configuration;
using Marvin.Container;
using Marvin.Logging;
using Marvin.Modules;
using Marvin.Modules.Server;
using Marvin.Runtime.ModuleManagement;
using Marvin.Runtime.ServerModules;

namespace Marvin.Runtime.Kernel.ModuleManagement
{
    /// <summary>
    /// Manages all the modules on the server side. 
    /// </summary>
    [InitializableKernelComponent(typeof(IModuleManager))]
    public class ModuleManager : IModuleManager, IInitializable, ILoggingHost
    {
        #region Injected Properties
        private IServerModule[] _serverModules = new IServerModule[0];
        /// <summary>
        /// Get/set the list of server modules. Injected by castle.
        /// </summary>
        public IServerModule[] ServerModules
        {
            get { return _serverModules; }
            set { _serverModules = value; }
        }

        /// <summary>
        /// Logger management instance. Injected by castle.
        /// </summary>
        public ILoggerManagement LoggerManagement { get; set; }
        /// <summary>
        /// Configuration manager instance. Injected by castel.
        /// </summary>
        public IConfigManager ConfigManager { get; set; }
        #endregion

        #region Fields
        private bool _eventsRegistered;

        private IModuleDependencyManager _dependencyManager;
        private IModuleStarter _moduleStarter;
        private IModuleStopper _moduleStopper;
        private IModuleFailureStrategy _failureStrategy;
        private ModuleManagerConfig _config;
        #endregion

        #region IModuleManager
        /// <summary>
        /// Initialize the module manager.
        /// </summary>
        public void Initialize()
        {
            // Create cross component objects
            var waitingServices = new Dictionary<IServerModule, ICollection<IServerModule>>();
            var allServices = new Func<IEnumerable<IServerModule>>(() => AllModules);

            // Create components
            LoggerManagement.ActivateLogging(this);
            _config = ConfigManager.GetConfiguration<ModuleManagerConfig>();
            _dependencyManager = new ModuleDependencyManager(Logger.GetChild(string.Empty, typeof(ModuleDependencyManager)));
            _moduleStarter = new ModuleStarter(_dependencyManager, Logger.GetChild(string.Empty, typeof(ModuleStarter)), _config);
            _moduleStopper = new ModuleStopper(_dependencyManager, Logger.GetChild(string.Empty, typeof(ModuleStopper)));
            _failureStrategy = new FailureStrategy(_config);

            // Link components
            var components = new IModuleManagerComponent[] { _dependencyManager, _moduleStarter, _moduleStopper };
            foreach (var component in components)
            {
                component.AllModules = allServices;
                component.WaitingModules = waitingServices;
            }

            // Build dependency tree
            _dependencyManager.BuildDependencyTree();

            // Link framework modules
            foreach (var platformModule in _serverModules.OfType<IPlatformModule>())
            {
                platformModule.SetModuleManager(this);
            }
        }

        private void RegisterEvents()
        {
            if (_eventsRegistered)
                return;

            foreach (var plugin in AllModules)
            {
                plugin.State.Changed += ModuleStateChanged;
            }
            _eventsRegistered = true;
        }

        /// <summary>
        /// Start all modules in cascading order
        /// </summary>
        public void StartModules()
        {
            RegisterEvents();
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
        /// Restart the module and all of its dependecies
        /// </summary>
        /// <param name="module"></param>
        public void ReincarnateModule(IServerModule module)
        {
            // Stop execution
            _moduleStopper.Stop(module);

            // Prepare for new start
            _moduleStarter.Initialize(module);

            // Start all desired
            _moduleStarter.Start(module);
        }

        /// <summary>
        /// Confirm error or warning for module
        /// </summary>
        /// <param name="module"></param>
        public void ConfirmAllNotifications(IServerModule module)
        {
            foreach (var notification in module.Notifications)
            {
                notification.Confirm();
            }
        }

        /// <summary>
        /// All modules
        /// </summary>
        public IEnumerable<IServerModule> AllModules
        {
            get { return _serverModules; }
        }

        /// <summary>
        /// Inform the listeners about module state changes. 
        /// </summary>
        public event EventHandler<ModuleStateChangedEventArgs> ModuleChangedState;

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
        public IDependencyEvaluation DependencyEvaluation
        {
            get { return _dependencyManager.GetDependencyEvalutaion(); }
        }

        /// <summary>
        /// Get or set a services behaviour using 
        /// </summary>
        /// <typeparam name="T">Type of behaviour</typeparam>
        public IBehaviourAccess<T> BehaviourAccess<T>(IServerModule plugin)
        {
            return ABehaviourAccess.Create<T>(_config, plugin);
        }

        #endregion

        private void ModuleStateChanged(object sender, ModuleStateChangedEventArgs eventArgs)
        {
            if (ModuleChangedState != null)
                ModuleChangedState(sender, eventArgs);

            ReincarnateOnFailure((IServerModule)sender, eventArgs.NewState);
        }

        private readonly IDictionary<IServerModule, int> _retryCount = new Dictionary<IServerModule, int>();
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

        /// <summary>
        /// Name of this host. Used for logger name structure
        /// </summary>
        public string Name { get { return "Kernel"; } }

        /// <summary>
        /// Logger instance
        /// </summary>
        public IModuleLogger Logger { get; set; }
    }
}
