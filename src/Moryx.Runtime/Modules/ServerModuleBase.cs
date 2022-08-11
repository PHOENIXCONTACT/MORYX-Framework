// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Modules;
using Moryx.Runtime.Container;
using Moryx.StateMachines;
using Moryx.Threading;
using LogLevel = Moryx.Logging.LogLevel;

namespace Moryx.Runtime.Modules
{
    /// <summary>
    /// Base class for a server module. Provides all necessary methods to be a server module.
    /// </summary>
    /// <typeparam name="TConf">Configuration type for the server module.</typeparam>
    [DebuggerDisplay("{" + nameof(Name) + "} - {" + nameof(State) + "}")]
    public abstract class ServerModuleBase<TConf> : IServerModule, IContainerHost, IServerModuleStateContext
        where TConf : class, IConfig, new()
    {
        /// <inheritdoc />
        public abstract string Name { get; }

        /// <summary>
        /// Logger of this module.
        /// </summary>
        public IModuleLogger Logger { get; set; }

        /// <inheritdoc />
        IServerModuleConsole IServerModule.Console => Container?.Resolve<IServerModuleConsole>();

        /// <inheritdoc />
        public INotificationCollection Notifications { get; } = new ServerNotificationCollection();

        /// <summary>
        /// Creates a new instance of <see cref="ServerModuleBase{TConf}"/> and initializes the state machine
        /// </summary>
        protected ServerModuleBase(IModuleContainerFactory containerFactory, IConfigManager configManager,IModuleLoggerFactory loggerFactory)
        {
            ContainerFactory = containerFactory;
            ConfigManager = configManager;
            Logger = loggerFactory.Create($"{GetType().Namespace}");
            Logger.SetNotificationTarget(Notifications.Add);

            StateMachine.Initialize((IServerModuleStateContext)this).With<ServerModuleStateBase>();
        }

        #region ValidateHealthState

        /// <summary>
        /// Does a validaton of the health state. Only states indicating <see cref="ServerModuleState.Running"/> will not
        /// throw an <see cref="HealthStateException"/>
        /// </summary>
        protected void ValidateHealthState() => _state.ValidateHealthState();

        #endregion

        #region Logging


        #endregion

        #region Server Module methods

        /// <summary>
        /// <see cref="IModuleContainerFactory"/>
        /// </summary>
        public IModuleContainerFactory ContainerFactory { get; }

        void IInitializable.Initialize()
        {
            lock (_stateLock)
                _state.Initialize();
        }
        void IServerModuleStateContext.Initialize()
        {
            // Activate logging
            Logger.Log(LogLevel.Info, "{0} is initializing...", Name);

            // Get config and parse for container settings
            Config = ConfigManager.GetConfiguration<TConf>();
            ConfigParser.ParseStrategies(Config, Strategies);

            // Initialize container with server module dll and this dll
            Container = ContainerFactory.Create(Strategies, GetType().Assembly)
                .Register<IParallelOperations, ParallelOperations>()
                // Register instances for this cycle
                .SetInstance(Config)
                .SetInstance(Logger);

            OnInitialize();

            // Execute SubInitializer
            var subInits = Container.ResolveAll<ISubInitializer>() ?? new ISubInitializer[0];
            foreach (var subInitializer in subInits)
            {
                subInitializer.Initialize(Container);
            }

            Logger.Log(LogLevel.Info, "{0} initialized!", Name);

            // After initializing the module, all notifications are unnecessary
            Notifications.Clear();
        }

        void IServerModule.Start()
        {
            lock (_stateLock)
                _state.Start();
        }

        void IServerModuleStateContext.Start()
        {
            Logger.Log(LogLevel.Info, "{0} is starting...", Name);

            OnStart();

            Logger.Log(LogLevel.Info, "{0} started!", Name);
        }

        void IServerModuleStateContext.Started()
        {
            OnStarted();
        }

        /// <summary>
        /// Called when module has been started
        /// </summary>
        protected internal virtual void OnStarted()
        {
        }

        void IServerModule.Stop()
        {
            lock (_stateLock)
                _state.Stop();
        }

        void IServerModuleStateContext.Stop()
        {
            Logger.Log(LogLevel.Info, "{0} is stopping...", Name);

            OnStop();

            Logger.Log(LogLevel.Info, "{0} stopped!", Name);
        }

        void IServerModuleStateContext.Destruct()
        {
            // Destroy local container
            if (Container != null)
            {
                Container.Destroy();
                Container = null;
            }

            Logger.Log(LogLevel.Info, "{0} destructed!", Name);
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
        public IConfigManager ConfigManager { get; }

        /// <summary>
        /// Config instance for the current lifecycle
        /// </summary>
        protected TConf Config { get; private set; }

        #endregion

        #region IServerModuleState

        private ServerModuleStateBase _state;

        private readonly object _stateLock = new object();

        void IStateContext.SetState(IState state)
        {
            var oldState = _state?.Classification ?? ServerModuleState.Stopped;

            _state = (ServerModuleStateBase)state;

            StateChange(oldState, State);
        }

        /// <summary>
        /// The current state.
        /// </summary>
        public ServerModuleState State => _state.Classification;

        /// <summary>
        /// Event is called when ModuleState changed
        /// </summary>
        public event EventHandler<ModuleStateChangedEventArgs> StateChanged;
        private void StateChange(ServerModuleState oldState, ServerModuleState newState)
        {
            if (StateChanged == null || oldState == newState)
            {
                return;
            }

            // Since event handling may take a while make sure we don't stop module execution
            foreach (var caller in StateChanged.GetInvocationList())
            {
                ThreadPool.QueueUserWorkItem(delegate (object callObj)
                {
                    try
                    {
                        var callDelegate = (Delegate)callObj;
                        callDelegate.DynamicInvoke(this, new ModuleStateChangedEventArgs { OldState = oldState, NewState = newState });
                    }
                    catch (Exception ex)
                    {
                        Logger?.LogException(LogLevel.Warning, ex, "Failed to notify listener of state change");
                    }
                }, caller);
            }
        }

        #endregion

        #region ErrorReporting

        /// <inheritdoc/>
        public void AcknowledgeNotification(IModuleNotification notification)
        {
            Notifications.Remove(notification);
        }

        /// <summary>
        /// Report internal failure to parent module
        /// </summary>
        void IServerModuleStateContext.ReportError(Exception exception)
        {
            // Add to log
            Logger.LogException(LogLevel.Fatal, exception, "Exception in module lifecycle!");
        }

        /// <inheritdoc />
        void IServerModuleStateContext.LogNotification(IModuleNotification notification)
        {
            Notifications.Add(notification);
        }

        #endregion
    }
}
