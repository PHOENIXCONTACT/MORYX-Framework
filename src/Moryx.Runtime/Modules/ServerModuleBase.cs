// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Modules;
using Moryx.Runtime.Container;
using Moryx.StateMachines;
using Moryx.Threading;

namespace Moryx.Runtime.Modules
{
    /// <summary>
    /// Base class for a server module. Provides all necessary methods to be a server module.
    /// </summary>
    /// <typeparam name="TConf">Configuration type for the server module.</typeparam>
    [DebuggerDisplay("{" + nameof(Name) + "} - {" + nameof(State) + "}")]
    public abstract class ServerModuleBase<TConf> : IServerModule, IServerModuleStateContext
        where TConf : class, IConfig, new()
    {
        /// <inheritdoc />
        public abstract string Name { get; }

        /// <summary>
        /// Logger of this module.
        /// </summary>
        public ILogger Logger { get; set; }

        public ILoggerFactory LoggerFactory { get; set; }

        /// <inheritdoc />
        IServerModuleConsole IServerModule.Console => Container?.Resolve<IServerModuleConsole>();

        /// <inheritdoc />
        public INotificationCollection Notifications { get; } = new ServerNotificationCollection();

        /// <summary>
        /// Creates a new instance of <see cref="ServerModuleBase{TConf}"/> and initializes the state machine
        /// </summary>
        protected ServerModuleBase(IConfigManager configManager, ILoggerFactory loggerFactory)
        {
            ConfigManager = configManager;
            LoggerFactory = loggerFactory;
            Logger = LoggerFactory.CreateLogger(GetType().Namespace);

            StateMachine.Initialize((IServerModuleStateContext)this).With<ServerModuleStateBase>();
        }

        #region ValidateHealthState

        /// <summary>
        /// Does a validaton of the health state. Only states indicating <see cref="ServerModuleState.Running"/> will not
        /// throw an <see cref="HealthStateException"/>
        /// </summary>
        protected void ValidateHealthState() => _state.ValidateHealthState();

        #endregion

        #region Server Module methods

        void IInitializable.Initialize()
        {
            lock (_stateLock)
                _state.Initialize();
        }
        void IServerModuleStateContext.Initialize()
        {
            // Activate logging
            var logger = new ModuleLogger(GetType().Namespace, LoggerFactory, Notifications.AddFromLogStream);

            Logger.Log(LogLevel.Information, "{0} is initializing...", Name);

            // Get config and parse for container settings
            Config = ConfigManager.GetConfiguration<TConf>();
            ConfigParser.ParseStrategies(Config, Strategies);

            // Initialize service collection with standard dependencies
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddFromAssembly(GetType().Assembly)
                .AddSingleton<IParallelOperations, ParallelOperations>()
                .AddSingleton(Config)
                .AddSingleton(LoggerFactory)
                .AddSingleton<IModuleLogger>(logger)
                .AddSingleton<ILogger>(logger);
            _container = new ServiceCollectionContainer(Strategies, serviceCollection);

            // Load all components from this assembly
            OnInitialize(serviceCollection);

            // Execute SubInitializer
            //var subInits = Container.ResolveAll<ISubInitializer>() ?? new ISubInitializer[0];
            //foreach (var subInitializer in subInits)
            //{
            //    subInitializer.Initialize(Container);
            //}

            _container.BuildProvider();

            Logger.Log(LogLevel.Information, "{0} initialized!", Name);

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
            Logger.Log(LogLevel.Information, "{0} is starting...", Name);

            OnStart(_container.ServiceProvider);

            Logger.Log(LogLevel.Information, "{0} started!", Name);
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
            Logger.Log(LogLevel.Information, "{0} is stopping...", Name);

            OnStop(_container.ServiceProvider);

            Logger.Log(LogLevel.Information, "{0} stopped!", Name);
        }

        void IServerModuleStateContext.Destruct()
        {
            // Destroy local container
            if (_container != null)
            {
                _container.Dispose();
                _container = null;
            }

            Logger.Log(LogLevel.Information, "{0} destructed!", Name);
        }

        #endregion

        #region Container

        private ServiceCollectionContainer _container;

        /// <summary>
        /// Internal container can only be set from inside this assembly
        /// </summary>
        [Obsolete("Try to replace direct access to the container with ServiceCollection and ServiceProvider calls")]
        public IContainer Container => _container;

        /// <summary>
        /// Configuration used for the container
        /// </summary>
        public IDictionary<Type, string> Strategies { get; } = new Dictionary<Type, string>();

        #endregion

        #region Child module transitions

        /// <summary>
        /// Code executed on start up and after service was stopped and should be started again.
        /// </summary>
        protected abstract void OnInitialize(IServiceCollection services);

        /// <summary>
        /// Code executed after OnInitialize
        /// </summary>
        protected abstract void OnStart(IServiceProvider serviceProvider);

        /// <summary>
        /// Code executed when service is stopped
        /// </summary>
        protected abstract void OnStop(IServiceProvider serviceProvider);

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
                        Logger?.Log(LogLevel.Warning, ex, "Failed to notify listener of state change");
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
            Logger.Log(LogLevel.Critical, exception, "Exception in module lifecycle!");
        }

        /// <inheritdoc />
        void IServerModuleStateContext.LogNotification(IModuleNotification notification)
        {
            Notifications.Add(notification);
        }

        #endregion
    }
}
