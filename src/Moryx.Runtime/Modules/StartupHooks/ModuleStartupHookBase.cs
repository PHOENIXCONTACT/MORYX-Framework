// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Tools;

namespace Moryx.Runtime.Modules.StartupHooks;

/// <summary>
/// Base class for StartupHooks that react to state changes of modules.
/// </summary>
/// <typeparam name="TFacade">The facade of the module. Is used to select the correct module</typeparam>
/// <typeparam name="TConfig">The type of config this hook requires</typeparam>
public abstract class ModuleStartupHookBase<TFacade, TConfig> : IStartupHook where TConfig : ConfigBase, new()
{
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
    /// Can be used to mark that the hooks should not be run.
    /// When the InitializationResult contains an error, the RunAsync Method will not register the event handler.
    /// </summary>
    protected FunctionResult InitializationResult { get; set; } = FunctionResult.Ok();

    /// <summary>
    /// Creates a ModuleStartupHookBase
    /// </summary>
    /// <param name="moduleManager">Used to access MORYX modules</param>
    /// <param name="configManager">Used to access the configuration</param>
    /// <param name="logger">logger</param>
    protected ModuleStartupHookBase(IModuleManager moduleManager, IConfigManager configManager, ILogger logger)
    {
        Logger = logger;
        ModuleManager = moduleManager;

        Config = configManager.GetConfiguration<TConfig>();
        if (Config is null)
        {
            InitializationResult = FunctionResult.WithError("Not configured");
        }
    }

    /// <summary>
    /// Method to handle the state change
    /// </summary>
    /// <param name="module">The module that changed its state</param>
    /// <param name="facade">The facade of the module</param>
    /// <param name="eventArgs">EventArgs with details about the change</param>
    protected abstract Task OnStateChanged(IServerModule module, TFacade facade, ModuleStateChangedEventArgs eventArgs);

    /// <inheritdoc/>
    public virtual Task RunAsync(CancellationToken cancellationToken = default)
    {
        if (!InitializationResult.Success)
        {
            if (InitializationResult.Error?.Exception is not null)
            {
                Logger.LogInformation(InitializationResult.Error.Exception, "Initialization failed: {message}. Not running this hook", InitializationResult.Error.Message);
            }
            else
            {
                Logger.LogInformation("Initialization failed: {message}. Not running this hook", InitializationResult.Error?.Message);
            }
            return Task.CompletedTask;
        }

        ModuleManager.ModuleStateChanged += OnModuleStateChanged;
        cancellationToken.Register(() => ModuleManager.ModuleStateChanged -= OnModuleStateChanged);

        return Task.CompletedTask;
    }

    private void OnModuleStateChanged(object sender, ModuleStateChangedEventArgs eventArgs)
    {
        _ = HandleStateChangedAsync(sender, eventArgs);
    }

    private async Task HandleStateChangedAsync(object sender, ModuleStateChangedEventArgs eventArgs)
    {
        if (sender is not (IServerModule module and IFacadeContainer<TFacade> facadeContainer))
        {
            return;
        }
        try
        {
            await OnStateChanged(module, facadeContainer.Facade, eventArgs);
        }
        // catch everything, because we don't want to interrupt the module state change
        catch (Exception ex)
        {
            Logger.LogError(ex, "OnStateChanged handler failed");
        }
    }
}
