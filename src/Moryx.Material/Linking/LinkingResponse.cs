// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Linking;

/// <summary>
/// Response returned by an <see cref="ILinkHandler{TReference}"/> after the request-phase hooks have executed.
/// </summary>
/// <typeparam name="TReference">Type of reference the link points to.</typeparam>
/// <param name="context">Validation context populated by all hooks.</param>
/// <param name="reference">Managed reference resolved for <see cref="LinkingRequest{TReference}.Target"/>, if any.</param>
public class LinkingResponse<TReference>(ValidationContext context, TReference? reference = null)
    where TReference : Reference
{
    /// <summary>
    /// Validation context populated by all hooks.
    /// </summary>
    public ValidationContext Context { get; } = context;

    /// <summary>
    /// True if no hook raised an error and the link can proceed (subject to requirement fulfillment).
    /// </summary>
    public bool IsAllowed => !Context.HasErrors;

    /// <summary>
    /// Managed reference resolved for the requested target. Applied to the link instead of the unresolved target.
    /// </summary>
    public TReference? Reference { get; } = reference;
}
