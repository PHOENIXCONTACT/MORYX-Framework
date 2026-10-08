// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Runtime.Modules;
using Moryx.Runtime.Kernel.Tests.Dummies;

namespace Moryx.Runtime.Kernel.Tests.ModuleMocks;

public class ServerModuleA : ServerModuleBase<RuntimeConfigManagerTestConfig2>, IFacadeContainer<IFacadeA>
{
    public ServerModuleA(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory)
        : base(containerFactory, configManager, loggerFactory)
    {
    }

    public override string Name => "ServerModuleA";

    public IFacadeA Facade { get; } = new FacadaA();

    public int InitializeCalls { get; private set; }

    public int StartCalls { get; private set; }

    public int StopCalls { get; private set; }

    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        InitializeCalls++;
        return Task.CompletedTask;
    }

    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        StartCalls++;
        return Task.CompletedTask;
    }

    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        StopCalls++;
        return Task.CompletedTask;
    }
}
