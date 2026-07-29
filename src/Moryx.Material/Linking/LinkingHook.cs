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
public abstract class LinkingHook : ILinkingHook
{
    /// <inheritdoc/>
    public required IMaterialContainer Container { protected get; set; }

    /// <inheritdoc/>
    public required LinkingRequest Request { protected get; set; }

    /// <inheritdoc/>
    public required ValidationContext ValidationContext { protected get; set; }

    /// <inheritdoc/>
    public Task HandleLinkRequestAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public Task HandleLinkAppliedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
