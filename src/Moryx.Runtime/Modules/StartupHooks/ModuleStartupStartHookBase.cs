// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;

namespace Moryx.Runtime.Modules.StartupHooks;

/// <summary>
/// Base class for StartupHooks that react to module starts.
/// </summary>
/// <typeparam name="TFacade">The facade of the Module. Is used to select the correct module</typeparam>
/// <typeparam name="TConfig">The type of config this hook requires</typeparam>
public abstract class ModuleStartupStartHookBase<TFacade, TConfig>(IModuleManager moduleManager, IConfigManager configManager, ILogger logger)
    : ModuleStartupHookBase<TFacade, TConfig>(moduleManager, configManager, logger) where TConfig : ConfigBase, new()
{
    private readonly SemaphoreSlim _semaphore = new(1);

    /// <summary>
    /// Will be true for the first run of OnModuleStarted and will be set to false afterward,
    /// regardless of the outcome of the call.
    /// </summary>
    protected bool FirstStart { get; set; } = true;

    /// <summary>
    /// Will be false until OnModuleStarted ran to completion without throwing an exception.
    /// </summary>
    protected bool SucceededAtLeastOnce { get; set; }

    /// <inheritdoc/>
    protected override async Task OnStateChanged(IServerModule module, TFacade facade, ModuleStateChangedEventArgs eventArgs)
    {
        if (eventArgs.NewState != ServerModuleState.Running)
        {
            return;
        }

        try
        {
            await _semaphore.WaitAsync();
            await OnModuleStarted(module, facade);
            SucceededAtLeastOnce = true;
        }
        finally
        {
            FirstStart = false;
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Called when the requested module was started
    /// </summary>
    /// <param name="module">Reference to the module</param>
    /// <param name="facade">Reference to the selected facade</param>
    /// <returns></returns>
    protected abstract Task OnModuleStarted(IServerModule module, TFacade facade);
}
