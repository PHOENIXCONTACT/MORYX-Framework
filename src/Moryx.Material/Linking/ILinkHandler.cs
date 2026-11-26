// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Linking;

/// <summary>
/// Handler of the two-phase linking protocol for a <see cref="MaterialContainerLink{TReference}"/>.
/// Integrations attach a handler to each link they manage.
/// </summary>
/// <typeparam name="TReference">Type of reference the link points to.</typeparam>
public interface ILinkHandler<TReference>
    where TReference : Reference
{
    /// <summary>
    /// Request phase: resolves the target, validates the request and returns the managed reference to apply.
    /// </summary>
    /// <param name="container">Container the link belongs to, as passed to <see cref="MaterialContainerLink{TReference}.AttachAsync"/>.</param>
    /// <param name="request">The linking request.</param>
    /// <param name="cancellationToken">Token used to cancel the validation.</param>
    Task<LinkingResponse<TReference>> ValidateAsync(IMaterialContainer container, LinkingRequest<TReference> request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Applied phase: called after the link applied the <paramref name="response"/>.
    /// </summary>
    /// <param name="container">Container the link belongs to, as passed to <see cref="MaterialContainerLink{TReference}.AttachAsync"/>.</param>
    /// <param name="request">The linking request.</param>
    /// <param name="response">The response returned by <see cref="ValidateAsync"/>.</param>
    /// <param name="cancellationToken">Token used to cancel the post-processing.</param>
    Task OnAppliedAsync(IMaterialContainer container, LinkingRequest<TReference> request, LinkingResponse<TReference> response,
        CancellationToken cancellationToken);

    // TODO: Add applied event for lineage processing?
}
