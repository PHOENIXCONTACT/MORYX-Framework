// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Orders.Integrator.Tests;

/// <summary>
/// Hook blocking every link request and counting the invocations of both phases.
/// </summary>
[Plugin(LifeCycle.Transient, typeof(ILinkingHook), Name = PluginName)]
public class BlockingOrderLinkingHook : OrderLinkingHook
{
    public const string PluginName = nameof(BlockingOrderLinkingHook);

    public static int RequestCount { get; set; }

    public static int AppliedCount { get; set; }

    public override Task HandleLinkRequestAsync(CancellationToken cancellationToken)
    {
        RequestCount++;
        if (!Request.IsUnlink)
        {
            ValidationContext.AddError($"{Target?.Number ?? "Unknown order"} is blocked.", GetType());
        }

        return Task.CompletedTask;
    }

    public override Task HandleLinkAppliedAsync(CancellationToken cancellationToken)
    {
        AppliedCount++;
        return Task.CompletedTask;
    }
}
