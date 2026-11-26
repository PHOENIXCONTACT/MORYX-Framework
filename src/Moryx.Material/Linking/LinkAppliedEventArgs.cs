// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Linking;

/// <summary>
/// Event args raised by a <see cref="MaterialContainerLink{TReference}"/> after a request was applied.
/// </summary>
/// <typeparam name="TReference">Type of reference the link points to.</typeparam>
/// <param name="container">Container the link belongs to.</param>
/// <param name="request">Original linking request.</param>
/// <param name="context">Validation context carried over from the request phase.</param>
/// <param name="appliedReference">The reference applied to the link, or <c>null</c> for an unlink.</param>
public class LinkAppliedEventArgs<TReference>(IMaterialContainer container, LinkingRequest<TReference> request,
    ValidationContext context, TReference? appliedReference) : EventArgs
    where TReference : Reference
{
    /// <summary>
    /// Container the link belongs to.
    /// </summary>
    public IMaterialContainer Container { get; } = container;

    /// <summary>
    /// Original linking request.
    /// </summary>
    public LinkingRequest<TReference> Request { get; } = request;

    /// <summary>
    /// Validation context carried over from the request phase.
    /// </summary>
    public ValidationContext Context { get; } = context;

    /// <summary>
    /// The reference applied to the link, or <c>null</c> for an unlink.
    /// </summary>
    public TReference? AppliedReference { get; } = appliedReference;
}
