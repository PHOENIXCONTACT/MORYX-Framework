// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Linking;

/// <summary>
/// Outcome of a link or unlink request on a <see cref="MaterialContainerLink"/>.
/// </summary>
public enum LinkingStatus
{
    /// <summary>The request was validated and applied.</summary>
    Applied,

    /// <summary>The request was rejected during validation; the link is unchanged.</summary>
    Rejected,

    /// <summary>No integration is attached yet; the request is stored and processed once one attaches.</summary>
    Pending,

    /// <summary>The request did not change anything, e.g. unlinking a link that is not linked.</summary>
    Unchanged
}

/// <summary>
/// Result of a link or unlink request on a <see cref="MaterialContainerLink"/>.
/// </summary>
/// <param name="status">Outcome of the request.</param>
/// <param name="context">Validation context populated during the request.</param>
public class LinkingResult(LinkingStatus status, ValidationContext context)
{
    /// <summary>
    /// Outcome of the request.
    /// </summary>
    public LinkingStatus Status { get; } = status;

    /// <summary>
    /// Validation context populated during the request. Empty for <see cref="LinkingStatus.Pending"/>
    /// and <see cref="LinkingStatus.Unchanged"/>.
    /// </summary>
    public ValidationContext Context { get; } = context;

    // TODO: Add shortcut static methods to create results for each enum case above
}
