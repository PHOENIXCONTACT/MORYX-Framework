// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;

namespace Moryx.Startup.Hooks;

public abstract class ModuleHook<TFacade>(IModuleManager moduleManager) : IStartupHook
{
    public int Priority { get; protected set; }
    protected IModuleManager ModuleManager { get; } = moduleManager;

    protected abstract Task OnStateChanged(IServerModule module, TFacade facade, ModuleStateChangedEventArgs eventArgs);

    public virtual async Task RunAsync()
    {
        ModuleManager.ModuleStateChanged += (sender, eventArgs) =>
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
