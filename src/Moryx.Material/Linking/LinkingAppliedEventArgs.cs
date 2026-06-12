// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Linking;

/// <summary>
/// Base event args raised by a container after the link has been applied (or unlinking completed).
/// </summary>
public abstract class LinkingAppliedEventArgs : EventArgs
{
    /// <summary>
    /// Container raising the event.
    /// </summary>
    public IMaterialContainer Container { get; }

    /// <summary>
    /// Original linking request.
    /// </summary>
    public LinkingRequest Request { get; }

    /// <summary>
    /// Validation context (carried over from the request phase).
    /// </summary>
    public ValidationContext Context { get; }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    protected LinkingAppliedEventArgs(IMaterialContainer container, LinkingRequest request, ValidationContext context)
    {
        Container = container;
        Request = request;
        Context = context;
    }
}