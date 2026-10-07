// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Configuration;
using Moryx.Container;
using Moryx.Runtime.Kernel.Tests.Dummies;
using Moryx.Runtime.Modules;
using Microsoft.Extensions.Logging;

namespace Moryx.Runtime.Kernel.Tests.ModuleMocks;

internal class ServerModuleADependent
    : ServerModuleBase<RuntimeConfigManagerTestConfig2>
{
    public ServerModuleADependent(
        IModuleContainerFactory containerFactory,
        IConfigManager configManager,
        ILoggerFactory loggerFactory)
        : base(containerFactory, configManager, loggerFactory)
    {
    }

    public override string Name => "ServerModuleADependent";

    [RequiredModuleApi(IsStartDependency = true)]
    public IFacadeA Dependency { get; set; }

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
