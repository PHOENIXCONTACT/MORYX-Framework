// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Linking;

/// <summary>
/// Base event args for a linking request raised by a container.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LinkingRequestEventArgs"/> class.
/// </remarks>
/// <param name="request">Linking request payload.</param>
/// <param name="responseCallback">Callback that will be invoked to deliver the <see cref="LinkingResponse"/> back to the sender.</param>
public abstract class LinkingRequestEventArgs(LinkingRequest request, Func<LinkingResponse, Task> responseCallback) : EventArgs
{
    /// <summary>
    /// Linking request payload.
    /// </summary>
    public LinkingRequest Request { get; } = request;

    /// <summary>
    /// Callback that will be invoked to deliver the <see cref="LinkingResponse"/> back to the <see cref="Container"/>
    /// </summary>
    public Func<LinkingResponse, Task> ResponseCallback { get; } = responseCallback;
}
