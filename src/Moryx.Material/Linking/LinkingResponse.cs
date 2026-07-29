// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Linking;

/// <summary>
/// Response returned to the container after the request-phase hooks have executed.
/// </summary>
public class LinkingResponse
{
    /// <summary>
    /// Validation context populated by all hooks.
    /// </summary>
    public ValidationContext Context { get; }

    /// <summary>
    /// True if no hook raised an error and the link can proceed (subject to requirement fulfillment).
    /// </summary>
    public bool IsAllowed => !Context.HasErrors;

    /// <summary>
    /// Optional reference object resolved during request handling (subclasses may carry typed payloads).
    /// </summary>
    public Reference? Reference { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinkingResponse"/> class.
    /// </summary>
    /// <param name="context">Validation context populated by all hooks.</param>
    /// <param name="reference">Optional reference object resolved during request handling.</param>
    public LinkingResponse(ValidationContext context, Reference? reference = null)
    {
        Context = context;
        Reference = reference;
    }
}
