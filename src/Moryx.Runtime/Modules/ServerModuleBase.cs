// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Modules;
using Moryx.StateMachines;
using Moryx.Threading;
using Moryx.Tools;

namespace Moryx.Runtime.Modules;

/// <summary>
/// Base class for a server module. Provides all necessary methods to be a server module.
/// </summary>
/// <typeparam name="TConf">Configuration type for the server module.</typeparam>
[DebuggerDisplay("{" + nameof(Name) + "} - {" + nameof(State) + "}")]
public abstract class ServerModuleBase<TConf> : IServerModule, IServerModuleStateContext
    where TConf : ConfigBase, new()
{
    /// <inheritdoc />
    public abstract string Name { get; }

    /// <summary>
    /// All facades that were activated
    /// </summary>
    private readonly ICollection<IFacadeControl> _activeFacades = new List<IFacadeControl>();

    /// <summary>
    /// Logger of this module.
    /// </summary>
    public ILogger Logger { get; set; }

    /// <summary>
    /// Shared factory to create logger in this module
    /// </summary>
    public ILoggerFactory LoggerFactory { get; set; }

    /// <inheritdoc />
    IServerModuleConsole IServerModule.Console => Container?.Resolve<IServerModuleConsole>();

    /// <inheritdoc />
    public INotificationCollection Notifications { get; } = new ServerNotificationCollection();

    /// <summary>
    /// Creates a new instance of <see cref="ServerModuleBase{TConf}"/> and initializes the state machine
    /// </summary>
    protected ServerModuleBase(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory)
    {
        ContainerFactory = containerFactory;
        ConfigManager = configManager;
        LoggerFactory = loggerFactory;
        Logger = LoggerFactory.CreateLogger(GetType().Namespace);

        StateMachine.ForAsyncContext((IServerModuleStateContext)this).WithAsync<ServerModuleStateBase>().Wait();
    }

    #region ValidateHealthState

    /// <summary>
    /// Does a validation of the health state. Only states indicating <see cref="ServerModuleState.Running"/> will not
    /// throw an <see cref="HealthStateException"/>
    /// </summary>
    protected void ValidateHealthState() => _state.ValidateHealthState();

    #endregion

    #region Server Module methods

    /// <summary>
    /// <see cref="IModuleContainerFactory"/>
    /// </summary>
    public IModuleContainerFactory ContainerFactory { get; }

    Task IAsyncInitializable.InitializeAsync(CancellationToken cancellationToken)
    {
        return _stateLockSemaphore.ExecuteAsync(() => _state.Initialize(cancellationToken), cancellationToken);
    }
    async Task IServerModuleStateContext.InitializeAsync(CancellationToken cancellationToken)
    {
        // Activate logging
        var logger = new ModuleLogger(GetType().Namespace, LoggerFactory, Notifications.AddFromLogStream);

        Logger.Log(LogLevel.Information, "{name} is initializing...", Name);

        // Get config and parse for container settings
        Config = ConfigManager.GetConfiguration<TConf>();
        ConfigParser.ParseStrategies(Config, Strategies);

        // Initialize container with server module dll and this dll
        Container = ContainerFactory.Create(Strategies, GetType().Assembly)
            .Register<IParallelOperations, ParallelOperations>()
            // Register instances for this cycle
            .SetInstance(Config)
            .SetInstance(LoggerFactory)
            .SetInstance<IModuleLogger>(logger, "ModuleLogger")
            .SetInstance<ILogger>(logger, "Logger");

        await OnInitializeAsync(cancellationToken);

        // Execute SubInitializer
        var subInitializers = Container.ResolveAll<ISubInitializer>() ?? [];
        foreach (var subInitializer in subInitializers)
        {
            subInitializer.Initialize(Container);
        }

        Logger.Log(LogLevel.Information, "{0} initialized!", Name);

        // After initializing the module, all notifications are unnecessary
        Notifications.Clear();
    }

    Task IServerModule.StartAsync(CancellationToken cancellationToken)
    {
        return _stateLockSemaphore.ExecuteAsync(() => _state.Start(cancellationToken), cancellationToken);
    }

    async Task IServerModuleStateContext.StartAsync(CancellationToken cancellationToken)
    {
        Logger.Log(LogLevel.Information, "{0} is starting...", Name);

        await OnStartAsync(cancellationToken);

        Logger.Log(LogLevel.Information, "{0} started!", Name);
    }

    void IServerModuleStateContext.Started()
    {
        foreach (var facade in _activeFacades.OfType<ILifeCycleBoundFacade>())
        {
            facade.Activated();
        }
    }

    Task IServerModule.StopAsync(CancellationToken cancellationToken)
    {
        return _stateLockSemaphore.ExecuteAsync(() => _state.Stop(cancellationToken), cancellationToken);
    }

    async Task IServerModuleStateContext.StopAsync(CancellationToken cancellationToken)
    {
        Logger.Log(LogLevel.Information, "{0} is stopping...", Name);

        await OnStopAsync(cancellationToken);

        Logger.Log(LogLevel.Information, "{0} stopped!", Name);
    }

    void IServerModuleStateContext.Destruct()
    {
        // Destroy local container
        if (Container != null)
        {
            Container.Destroy();
            Container = null;
        }

        Logger.Log(LogLevel.Information, "{0} destructed!", Name);
    }

    #endregion

    #region Facade

    /// <summary>
    /// Activate our public API facade and link all dependencies into the local container
    /// </summary>
    protected void ActivateFacade(IFacadeControl facade)
    {
        // First activation
        facade.ValidateHealthState = ValidateHealthState;

        FillProperties(facade, FillProperty);
        facade.Activate();

        _activeFacades.Add(facade);
    }

    /// <summary>
    /// Deactivate our public facade and remove all references into the container
    /// </summary>
    protected void DeactivateFacade(IFacadeControl facade)
    {
        if (!_activeFacades.Remove(facade))
            return;

        facade.Deactivate();
        FillProperties(facade, (a, b) => null);

        var lifeCycleBoundFacade = facade as ILifeCycleBoundFacade;
        lifeCycleBoundFacade?.Deactivated();
    }

    private void FillProperties(object instance, Func<IContainer, PropertyInfo, object> fillingFunc)
    {
        // Fill everything available in the container
        foreach (var prop in instance.GetType().GetProperties())
        {
            var type = prop.PropertyType;
            type = typeof(Array).IsAssignableFrom(type) ? type.GetElementType() : type;
            var implementations = Container.GetRegisteredImplementations(type);
            if (!implementations.Any())
                continue;

            if (prop.SetMethod == null)
                continue;

            prop.SetValue(instance, fillingFunc(Container, prop));
        }
    }

    private object FillProperty(IContainer container, PropertyInfo property)
    {
        var propType = property.PropertyType;
        if (typeof(Array).IsAssignableFrom(propType))
            return container.ResolveAll(propType.GetElementType());

        var strategyName = Strategies.TryGetValue(propType, out var strategy) ? strategy : null;
        return strategyName == null ? Container.Resolve(propType)
            : Container.Resolve(propType, strategyName);
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
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    protected abstract Task OnInitializeAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Code executed after OnInitialize
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    protected abstract Task OnStartAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Code executed when service is stopped
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    protected abstract Task OnStopAsync(CancellationToken cancellationToken);

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

    private readonly SemaphoreSlim _stateLockSemaphore = new(1, 1);

    Task IAsyncStateContext.SetStateAsync(StateBase state, CancellationToken cancellationToken)
    {
        var oldState = _state?.Classification ?? ServerModuleState.Stopped;

        _state = (ServerModuleStateBase)state;

        StateChange(oldState, State);

        return Task.CompletedTask;
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
                    Logger.Log(LogLevel.Warning, ex, "Failed to notify listener of state change");
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