// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Moryx.Configuration;
using Moryx.Container;
using Microsoft.Extensions.Logging;

namespace Moryx.Runtime.Kernel.Tests.ModuleMocks;

internal class BlockingInitializeModule : MockServerModuleBase 
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


    protected override async Task OnInitializeAsync(
        CancellationToken cancellationToken)
    {
        InitializeEntered.TrySetResult();

        await ContinueInitialize.Task;
    }

}
