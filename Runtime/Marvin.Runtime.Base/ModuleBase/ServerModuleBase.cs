using System;
using System.Collections.Generic;
using System.Linq;
using Marvin.Configuration;
using Marvin.Container;
using Marvin.Logging;
using Marvin.Modules;
using Marvin.Runtime.Base.HealthState;
using Marvin.Runtime.Container;
using Marvin.Runtime.ModuleManagement;
using Marvin.Threading;
using Marvin.Tools;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.Base
{
    /// <summary>
    /// Base class for a server module. Provides all necessary methods to be a server module.  
    /// </summary>
    /// <typeparam name="TConf">Configuration type for the server module.</typeparam>
    public abstract class ServerModuleBase<TConf> : IServerModule, IContainerHost, IStateBasedTransitions, ILoggingHost
        where TConf : class, IConfig, new()
    {
        private readonly HealthStateController _stateController;

        /// <summary>
        /// Constructor for this class which initialize all necessary instances.
        /// </summary>
        protected ServerModuleBase()
        {
            _stateController = new HealthStateController(this);
        }

        /// <summary>
        /// Unique name for a module within the platform it is designed for
        /// </summary>
        public abstract string Name { get; }

        IServerModuleConsole IServerModule.Console => Container?.Resolve<IServerModuleConsole>();

        /// <summary>
        /// Notifications raised within module and during state changes.
        /// </summary>
        public INotificationCollection Notifications => _stateController.Notifications;

        #region Server Module states

        /// <summary>
        /// Access to the modules internal state
        /// </summary>
        IServerModuleState IServerModule.State => _stateController;

        /// <summary>
        /// Does a validaton of the health state with all states which have the running flag. <see cref="ServerModuleState"/> for the states.
        /// </summary>
        protected void ValidateHealthState()
        {
            var requiredStates = Enum.GetValues(typeof(ServerModuleState)).Cast<ServerModuleState>()
                .Where(state => state.HasFlag(ServerModuleState.Running)).ToArray();
            ValidateHealthState(requiredStates);
        }
        /// <summary>
        /// Validates the health state to find out, if the module is in one of the required states. 
        /// </summary>
        /// <param name="requiredStates">Array of the states where the module can be in.</param>
        /// <exception cref="HealthStateException">When module is not in one of the required states.</exception>
        protected void ValidateHealthState(ServerModuleState[] requiredStates)
        {
            if (!requiredStates.Any(state => state == _stateController.Current))
                throw new HealthStateException(_stateController.Current, requiredStates);
        }
        #endregion

        #region Logging

        /// <summary>
        /// <see cref="ILoggerManagement"/> 
        /// </summary>
        public ILoggerManagement LoggerManagement { get; set; }

        /// <summary>
        /// Logger of the current state.
        /// </summary>
        public IModuleLogger Logger
        {
            get { return _stateController.Logger; }
            set { _stateController.Logger = value; }
        }

        #endregion

        #region Server Module methods

        /// <summary>
        /// <see cref="IModuleContainerFactory"/>
        /// </summary>
        public IModuleContainerFactory ContainerFactory { get; set; }


        void IInitializable.Initialize()
        {
            _stateController.Initialize();
        }
        void IStateBasedTransitions.Initialize()
        {
            // Activate logging
            LoggerManagement.ActivateLogging(this);
            _stateController.Logger = Logger;
            Logger.LogEntry(LogLevel.Info, "{0} is initializing...", Name);

            // Get config and parse for container settings
            Config = ConfigManager.GetConfiguration<TConf>();
            ConfigParser.ParseStrategies(Config, Strategies);

            // Initizalize container with server module dll and this dll
            Container = ContainerFactory.Create(Strategies, GetType().Assembly)
                .SetInstance<IModuleErrorReporting>(_stateController)
                .Register<IParallelOperations, ParallelOperations>()
                // Register instances for this cycle
                .SetInstance(Config).SetInstance(Logger);

            // Activate WCF
            EnableWcf(HostFactory);

            OnInitialize();

            var subInits = Container.ResolveAll<ISubInitializer>() ?? new ISubInitializer[0];
            foreach (var subInitializer in subInits)
            {
                subInitializer.Initialize(Container);
            }

            Logger.LogEntry(LogLevel.Info, "{0} initialized!", Name);
        }

        void IServerModule.Start()
        {
            _stateController.Start();
        }
        void IStateBasedTransitions.Start()
        {
            Logger.LogEntry(LogLevel.Info, "{0} is starting...", Name);

            OnStart();

            Logger.LogEntry(LogLevel.Info, "{0} started!", Name);
        }

        void IServerModule.Stop()
        {
            _stateController.Stop();
        }
        void IStateBasedTransitions.Stop()
        {
            Logger.LogEntry(LogLevel.Info, "{0} is stopping...", Name);

            OnStop();

            // Destroy local container
            if (Container != null)
            {
                Container.Destroy();
                Container = null;
            }
            // Deregister from logging
            Logger.LogEntry(LogLevel.Info, "{0} stopped!", Name);
            LoggerManagement.DeactivateLogging(this);
            _stateController.Logger = Logger;
        }
        #endregion

        #region Container
        /// <summary>
        /// Internal container can only be set from inside this assembly
        /// </summary>
        public IContainer Container { get; private set; }

        /// <summary>
        /// Configuration used for the container
        /// </summary>
        public IDictionary<Type, string> Strategies { get; } = new Dictionary<Type, string>();

        /// <summary>
        /// Wcf host factory to open wcf services
        /// </summary>
        public IWcfHostFactory HostFactory { get; set; }

        /// <summary>
        /// Enable wcf for this module
        /// </summary>
        private void EnableWcf(IWcfHostFactory hostFactory)
        {
            var typedFactory = Container.Resolve<ITypedHostFactory>();
            Container.SetInstance((IConfiguredHostFactory)new ConfiguredHostFactory(hostFactory)
            {
                Factory = typedFactory,
                Logger = Logger
            });
        }
        #endregion

        #region Child module transitions
        /// <summary>
        /// Code executed on start up and after service was stopped and should be started again.
        /// </summary>
        protected abstract void OnInitialize();

        /// <summary>
        /// Code executed after OnInitialize
        /// </summary>
        protected abstract void OnStart();

        /// <summary>
        /// Code executed when service is stopped
        /// </summary>
        protected abstract void OnStop();
        #endregion

        #region Configuration

        /// <summary>
        /// Config manager kernel component used to access this module config
        /// </summary>
        public IConfigManager ConfigManager { get; set; }

        /// <summary>
        /// Config instance for the current lifecycle
        /// </summary>
        protected TConf Config { get; private set; }

        #endregion

        /// <summary>
        /// Override to provide specific information about the server module.
        /// </summary>
        /// <returns>Name and current health state of the module.d</returns>
        public override string ToString()
        {
            return $"{Name} - {_stateController.Current}";
        }
    }
}
