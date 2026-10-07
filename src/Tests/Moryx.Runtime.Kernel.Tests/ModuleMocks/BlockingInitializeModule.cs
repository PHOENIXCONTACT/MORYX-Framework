// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Runtime.Kernel.Tests.Dummies;
using Moryx.Runtime.Modules;
using Microsoft.Extensions.Logging;

namespace Moryx.Runtime.Kernel.Tests.ModuleMocks;

internal class BlockingInitializeModule
    : ServerModuleBase<RuntimeConfigManagerTestConfig2>
{
    public BlockingInitializeModule(
        IModuleContainerFactory containerFactory,
        IConfigManager configManager,
        ILoggerFactory loggerFactory)
        : base(containerFactory, configManager, loggerFactory)
    {
    }

    public override string Name => "BlockingInitializeModule";

    public TaskCompletionSource InitializeEntered { get; } = new();

    public TaskCompletionSource ContinueInitialize { get; } = new();

    public int StartCalls { get; private set; }

    protected override async Task OnInitializeAsync(
        CancellationToken cancellationToken)
    {
        InitializeEntered.TrySetResult();

        await ContinueInitialize.Task;
    }

    protected override Task OnStartAsync(
        CancellationToken cancellationToken)
    {
        StartCalls++;
        return Task.CompletedTask;
    }

    protected override Task OnStopAsync(
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
