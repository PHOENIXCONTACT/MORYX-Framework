// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Linking;

/// <summary>
/// Plugin base class for hooks that participate in the two-phase linking protocol.
/// </summary>
/// <remarks>
/// Hooks are instantiated transiently per linking request by the orchestrating handler.
/// They observe and may influence the validation phase via the <see cref="ValidationContext"/>,
/// and produce side effects in the applied phase. Information about other links of the
/// <see cref="Container"/> is available through <see cref="IMaterialContainer.Links"/>.
/// </remarks>
/// <typeparam name="TReference">Type of reference the handled link points to.</typeparam>
/// <typeparam name="TBusinessObject">Type of business object resolved for the reference.</typeparam>
public abstract class LinkingHook<TReference, TBusinessObject> : ILinkingHook
    where TReference : Reference
    where TBusinessObject : class
{
    /// <summary>
    /// Container the link belongs to. Set by the orchestrator before invocation.
    /// </summary>
    public required IMaterialContainer Container { protected get; set; }

    /// <summary>
    /// Linking request being handled. Set by the orchestrator before invocation.
    /// </summary>
    public required LinkingRequest<TReference> Request { protected get; set; }

    /// <inheritdoc/>
    public required ValidationContext ValidationContext { protected get; set; }

    /// <summary>
    /// The resolved business object being linked. <c>null</c> when handling a pure unlink request
    /// or if the business object could not be resolved.
    /// </summary>
    public TBusinessObject? Target { protected get; set; }

    /// <summary>
    /// The previously linked business object, if any.
    /// </summary>
    public TBusinessObject? Previous { protected get; set; }

    /// <inheritdoc/>
    public virtual Task HandleLinkRequestAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public virtual Task HandleLinkAppliedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
