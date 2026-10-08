// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Runtime.Kernel.Tests.Dummies;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel.Tests.ModuleMocks;

public abstract class MockServerModuleBase : ServerModuleBase<RuntimeConfigManagerTestConfig2>
{
    protected MockServerModuleBase(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory)
        : base(containerFactory, configManager, loggerFactory)
    {
    }

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
