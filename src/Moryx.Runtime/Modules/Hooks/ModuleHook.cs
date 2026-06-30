// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Tools;

namespace Moryx.Runtime.Modules.Hooks;

/// <summary>
/// Base class for StarupHooks that react to state changes of modules.
/// </summary>
/// <typeparam name="TFacade">The facade of the module. Is used to select the correct module</typeparam>
/// <typeparam name="TConfig">The type of config this hook requires</typeparam>
public abstract class ModuleHook<TFacade, TConfig> : IStartupHook where TConfig : ConfigBase, new()
{
    /// <summary>
    /// Config for this hook. Retrieved from the ConfigManager by default
    /// </summary>
    protected readonly TConfig _config;

    /// <summary>
    /// Logger to record actions and problems
    /// </summary>
    protected readonly ILogger _logger;

    /// <summary>
    /// <see cref="IModuleManager"/>
    /// </summary>
    protected readonly IModuleManager _moduleManager;

    /// <summary>
    /// Defines when the hook runs. Lower priorities run earlier
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Can be used to mark that the hooks should not be run.
    /// When the InitializationResult contains an error, the RunAsync Method will not register the event handler.
    /// </summary>
    protected FunctionResult InitializationResult { get; set; } = FunctionResult.Ok();

    /// <summary>
    /// Creates a ModuleHook
    /// </summary>
    /// <param name="moduleManager">Used to access MORYX modules</param>
    /// <param name="configManager">Used to access the configuration</param>
    /// <param name="logger">logger</param>
    public ModuleHook(IModuleManager moduleManager, IConfigManager configManager, ILogger logger)
    {
        _logger = logger;
        _moduleManager = moduleManager;

        _config = configManager.GetConfiguration<TConfig>();
        if (_config is null)
        {
            InitializationResult = FunctionResult.WithError("Not configured");
        }
    }

    /// <summary>
    /// Method to handle the state change
    /// </summary>
    /// <param name="module">The module that changed it's state</param>
    /// <param name="facade">The facade of the module</param>
    /// <param name="eventArgs">EventArgs with detials about the change</param>
    protected abstract Task OnStateChanged(IServerModule module, TFacade facade, ModuleStateChangedEventArgs eventArgs);

    /// <inheritdoc/>
    public virtual async Task RunAsync(CancellationToken cancellationToken)
    {
        if (!InitializationResult.Success)
        {
            if (InitializationResult.Error.Exception is Exception ex)
            {
                _logger.LogInformation(ex, "Initialization failed: {message}. Not running this hook", InitializationResult.Error.Message);
            }
            else
            {
                _logger.LogInformation("Initialization failed: {message}. Not running this hook", InitializationResult.Error.Message);
            }
            return;
        }

        _moduleManager.ModuleStateChanged += (sender, eventArgs) =>
        {
            Task.Run(async () =>
            {
                if (sender is not IServerModule module || module is not IFacadeContainer<TFacade> facadeContainer)
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
                    _logger.LogError(ex, "OnStateChanged handler failed");
                }
            }).GetAwaiter().GetResult();
        };
    }
}
