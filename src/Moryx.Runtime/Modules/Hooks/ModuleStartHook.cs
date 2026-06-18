// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;

namespace Moryx.Runtime.Modules.Hooks;

/// <summary>
/// Base class for StarupHooks that react to module starts.
/// </summary>
/// <typeparam name="TFacade">The facade of the Module. Is used to select the correct module</typeparam>
/// <typeparam name="TConfig">The type of config this hook requires</typeparam>
public abstract class ModuleStartHook<TFacade, TConfig>(IModuleManager moduleManager, IConfigManager configManager, ILogger logger)
    : ModuleHook<TFacade, TConfig>(moduleManager, configManager, logger) where TConfig : ConfigBase, new()
{

    /// <summary>
    /// Will be true for the first run of OnModuleStarted and will be set to false afterwards,
    /// regardless of the outcome of the call.
    /// </summary>
    protected bool _firstStart = true;

    /// <summary>
    /// Will be false until OnModuleStarted ran to completion without throwing an exception.
    /// </summary>
    protected bool _succeededAtLeastOnce;

    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

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
            _succeededAtLeastOnce = true;
        }
        catch (Exception ex)
        {
            base._logger.LogError(ex, "OnModuleStarted handler failed");
        }
        finally
        {
            _firstStart = false;
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
