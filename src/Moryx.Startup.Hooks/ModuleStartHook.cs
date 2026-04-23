// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moryx.Runtime.Modules;
using Moryx.Tools;

namespace Moryx.Startup.Hooks;

public abstract class ModuleStartHook<TFacade, TConfig> : ModuleHook<TFacade>
     where TConfig : class
{

    protected bool _firstStart = true;
    protected bool _succeededAtLeastOnce;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

    public ModuleStartHook(IModuleManager moduleManager, IConfiguration configuration, string section, ILogger logger) : base(moduleManager)
    {
        _logger = logger;
        config = configuration.GetSection(section)?.Get<TConfig>();
        if (config is null)
        {
            InitResult = FunctionResult.WithError("Not configured");
            return;
        }

        InitResult = Initialize(config);
    }
    private readonly TConfig? config;
    private readonly ILogger _logger;

    public FunctionResult InitResult { get; }

    public override async Task RunAsync()
    {
        if (!InitResult.Success)
        {
            return;
        }

        await base.RunAsync();
    }

    protected virtual FunctionResult Initialize(TConfig config)
    {
        return FunctionResult.Ok();
    }

    protected override async Task OnStateChanged(IServerModule module, TFacade facade, ModuleStateChangedEventArgs eventArgs)
    {
        if (eventArgs.NewState != ServerModuleState.Running)
        {
            return;
        }

        try
        {
            await _semaphore.WaitAsync();
            await OnModuleStarted(module, facade, config!);
            _succeededAtLeastOnce = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to OnModuleStarted handler");
        }
        finally
        {
            _firstStart = false;
            _semaphore.Release();
        }
    }

    protected abstract Task OnModuleStarted(IServerModule module, TFacade facade, TConfig config);
}
