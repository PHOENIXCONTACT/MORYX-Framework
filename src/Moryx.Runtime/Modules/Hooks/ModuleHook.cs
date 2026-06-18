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
    public ModuleHook(IModuleManager moduleManager, IConfigManager configManager, ILogger logger)
    {
        _logger = logger;
        _moduleManager = moduleManager;

        _config = configManager.GetConfiguration<TConfig>();
        if (_config is null)
        {
            InitResult = FunctionResult.WithError("Not configured");
            return;
        }

        InitResult = Initialize(_config);
    }

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

    protected FunctionResult InitResult { get; }

    public int Priority { get; set; }

    protected virtual FunctionResult Initialize(TConfig config)
    {
        return FunctionResult.Ok();
    }

    protected abstract Task OnStateChanged(IServerModule module, TFacade facade, ModuleStateChangedEventArgs eventArgs);

    public virtual async Task RunAsync()
    {
        if (!InitResult.Success)
        {
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
                await OnStateChanged(module, facadeContainer.Facade, eventArgs);
            }).GetAwaiter().GetResult();
        };
    }
}
