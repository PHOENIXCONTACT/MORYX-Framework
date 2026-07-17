// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Tools;

namespace Moryx.Runtime.Modules;

/// <summary>
/// Base class for LifecycleHooks that react to a specific module state.
/// </summary>
/// <typeparam name="TFacade">The facade of the module. Is used to select the correct module</typeparam>
/// <typeparam name="TConfig">The type of config this hook requires</typeparam>
public abstract class ModuleLifecycleHookBase<TFacade, TConfig> : ILifecycleHook, IDisposable where TConfig : ConfigBase, new()
{
    private readonly SemaphoreSlim _semaphore = new(1);

    /// <summary>
    /// Config for this hook. Retrieved from the ConfigManager by default
    /// </summary>
    protected TConfig Config { get; }

    /// <summary>
    /// Logger to record actions and problems
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// <see cref="IModuleManager"/>
    /// </summary>
    protected IModuleManager ModuleManager { get; }

    /// <summary>
    /// Defines when the hook runs. Lower priorities run earlier
    /// </summary>
    public int Priority { get; init; }

    /// <summary>
    /// If set, the hook will not run and the reason will be logged.
    /// </summary>
    protected string SkipReason { get; set; }

    /// <summary>
    /// The module states this hook reacts to
    /// </summary>
    protected abstract ServerModuleState[] TargetStates { get; }

    /// <summary>
    /// Will be true for the first run of <see cref="OnTargetStateReached"/> and will be set to false afterward,
    /// regardless of the outcome of the call.
    /// </summary>
    protected bool FirstRun { get; set; } = true;

    /// <summary>
    /// Will be false until <see cref="OnTargetStateReached"/> ran to completion without throwing an exception.
    /// </summary>
    protected bool SucceededAtLeastOnce { get; set; }

    /// <summary>
    /// Creates a ModuleLifecycleHookBase
    /// </summary>
    /// <param name="moduleManager">Used to access MORYX modules</param>
    /// <param name="configManager">Used to access the configuration</param>
    /// <param name="logger">Logger for this component</param>
    protected ModuleLifecycleHookBase(IModuleManager moduleManager, IConfigManager configManager, ILogger logger)
    {
        Logger = logger;
        ModuleManager = moduleManager;
        Config = configManager.GetConfiguration<TConfig>();
    }

    /// <inheritdoc/>
    public virtual Task RunAsync(CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(SkipReason))
        {
            Logger.LogInformation("Not running this lifecycle hook: {reason}", SkipReason);
            return Task.CompletedTask;
        }

        ModuleManager.ModuleStateChanged += OnModuleStateChanged;

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual void Dispose()
    {
        ModuleManager.ModuleStateChanged -= OnModuleStateChanged;
        GC.SuppressFinalize(this);
    }

    private void OnModuleStateChanged(object sender, ModuleStateChangedEventArgs eventArgs)
    {
        // Encapsulate from event
        Task.Run(() => HandleStateChangedAsync(sender, eventArgs));
    }

    private async Task HandleStateChangedAsync(object sender, ModuleStateChangedEventArgs eventArgs)
    {
        if (sender is not (IServerModule module and IFacadeContainer<TFacade> facadeContainer))
        {
            return;
        }

        if (!TargetStates.Contains(eventArgs.NewState))
        {
            return;
        }

        try
        {
            await _semaphore.WaitAsync();
            await OnTargetStateReached(module, facadeContainer.Facade, eventArgs.NewState);
            SucceededAtLeastOnce = true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "HandleStateChangedAsync handler failed");
        }
        finally
        {
            FirstRun = false;
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Called when the requested module reached one of the <see cref="TargetStates"/>
    /// </summary>
    /// <param name="module">Reference to the module</param>
    /// <param name="facade">Reference to the selected facade</param>
    /// <param name="state">The module state that triggered this call</param>
    protected abstract Task OnTargetStateReached(IServerModule module, TFacade facade, ServerModuleState state);
}
