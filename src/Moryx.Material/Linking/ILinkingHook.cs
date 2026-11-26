// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Linking;

/// <summary>
/// Plugin interface for hooks that participate in the two-phase linking protocol.
/// </summary>
/// <remarks>
/// Hooks are instantiated transiently per linking request by the orchestrating handler.
/// They observe and may influence the validation phase via the <see cref="ValidationContext"/>,
/// and produce side effects in the applied phase. Implementations should derive from
/// <see cref="LinkingHook{TReference, TBusinessObject}"/>.
/// </remarks>
public interface ILinkingHook
{
    /// <summary>
    /// Shared validation context. Set by the orchestrator before invocation.
    /// </summary>
    ValidationContext ValidationContext { set; }

    /// <summary>
    /// Called during the request phase. Hooks may add validation entries or requirements.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the asynchronous hook invocation.</param>
    /// <returns>A task representing the asynchronous hook invocation.</returns>
    Task HandleLinkRequestAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Called during the applied phase to perform side effects (notifications, tracking, etc.).
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the asynchronous hook invocation.</param>
    /// <returns>A task representing the asynchronous hook invocation.</returns>
    Task HandleLinkAppliedAsync(CancellationToken cancellationToken);
}
