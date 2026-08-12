// Copyright (c) 2026 Phoenix ContacancellationToken GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Linking;

// TODO: Does configurable plugin creation work with a base class instead of an interface?
/// <summary>
/// Plugin interface for hooks that participate in the two-phase linking protocol.
/// </summary>
/// <remarks>
/// Hooks are instantiated transiently per linking request by the orchestrating manager.
/// They observe and may influence the validation phase via the <see cref="ValidationContext"/>,
/// and produce side effecancellationTokens in the applied phase.
/// </remarks>
public interface ILinkingHook
{
    /// <summary>
    /// Container raising the linking event. Set by the orchestrator before invocation.
    /// </summary>
    IMaterialContainer Container { set; }

    /// <summary>
    /// Linking request being handled. Set by the orchestrator before invocation.
    /// </summary>
    LinkingRequest Request { set; }

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
    /// Called during the applied phase to perform side effecancellationTokens (notifications, tracking, etc.).
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the asynchronous hook invocation.</param>
    /// <returns>A task representing the asynchronous hook invocation.</returns>
    Task HandleLinkAppliedAsync(CancellationToken cancellationToken);
}
