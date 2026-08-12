// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Linking;

/// <summary>
/// Base event args raised by a container after the link has been applied (or unlinking completed).
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LinkingAppliedEventArgs"/> class.
/// </remarks>
/// <param name="request">Original linking request.</param>
/// <param name="context">Validation context carried over from the request phase.</param>
public abstract class LinkingAppliedEventArgs(LinkingRequest request, ValidationContext context) : EventArgs
{
    /// <summary>
    /// Original linking request.
    /// </summary>
    public LinkingRequest Request { get; } = request;

    /// <summary>
    /// Validation context (carried over from the request phase).
    /// </summary>
    public ValidationContext Context { get; } = context;
}
