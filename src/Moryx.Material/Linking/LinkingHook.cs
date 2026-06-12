// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Linking;

/// <summary>
/// Plugin base class for hooks that participate in the two-phase linking protocol.
/// </summary>
/// <remarks>
/// Hooks are instantiated transiently per linking request by the orchestrating manager.
/// They observe and may influence the validation phase via the <see cref="ValidationContext"/>,
/// and produce side effects in the applied phase.
/// </remarks>
public abstract class LinkingHook
{
    /// <summary>
    /// Container raising the linking event. Set by the orchestrator before invocation.
    /// </summary>
    protected internal IMaterialContainer Container { get; internal set; } = null!;

    /// <summary>
    /// Linking request being handled. Set by the orchestrator before invocation.
    /// </summary>
    protected internal LinkingRequest Request { get; internal set; } = null!;

    /// <summary>
    /// Shared validation context. Set by the orchestrator before invocation.
    /// </summary>
    protected internal ValidationContext ValidationContext { get; internal set; } = null!;

    /// <summary>
    /// Called during the request phase. Hooks may add validation entries or requirements.
    /// </summary>
    public virtual Task HandleLinkRequestAsync(CancellationToken ct) => Task.CompletedTask;

    /// <summary>
    /// Called during the applied phase to perform side effects (notifications, tracking, etc.).
    /// </summary>
    public virtual Task HandleLinkAppliedAsync(CancellationToken ct) => Task.CompletedTask;
}