// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Linking;

/// <summary>
/// Linking request payload created by a <see cref="MaterialContainerLink{TReference}"/>.
/// </summary>
/// <typeparam name="TReference">Type of reference the link points to.</typeparam>
/// <param name="target">Reference that should be linked; <c>null</c> for a pure unlinking request.</param>
/// <param name="previous">Reference linked at the time of the request, if any.</param>
public class LinkingRequest<TReference>(TReference? target, TReference? previous)
    where TReference : Reference
{
    /// <summary>
    /// Reference that should be linked. <c>null</c> if this is a pure unlinking request.
    /// The reference is unresolved, i.e. it only carries the identifying information.
    /// </summary>
    public TReference? Target { get; } = target;

    /// <summary>
    /// Reference linked at the time of the request, if any. It is unlinked as part of a (re)link.
    /// </summary>
    public TReference? Previous { get; } = previous;

    /// <summary>
    /// Whether the request is for unlinking only (i.e. clearing the existing link).
    /// </summary>
    public bool IsUnlink => Target is null;
}
