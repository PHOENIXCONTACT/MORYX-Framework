// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Linking;

/// <summary>
/// Base event args for a linking request raised by a container.
/// </summary>
public abstract class LinkingRequestEventArgs : EventArgs
{
    /// <summary>
    /// Container raising the event.
    /// </summary>
    public IMaterialContainer Container { get; }

    /// <summary>
    /// Linking request payload.
    /// </summary>
    public LinkingRequest Request { get; }

    /// <summary>
    /// Callback the manager will invoke to deliver the response back to the container.
    /// </summary>
    public Func<LinkingResponse, Task>? ResponseCallback { get; set; }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    protected LinkingRequestEventArgs(IMaterialContainer container, LinkingRequest request)
    {
        Container = container;
        Request = request;
    }
}