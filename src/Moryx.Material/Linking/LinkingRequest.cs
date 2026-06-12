// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Linking;

/// <summary>
/// Base class for linking request payloads emitted by containers.
/// </summary>
/// <remarks>
/// Domain-specific integrations subclass this with their reference data
/// (e.g. <c>OrderLinkingRequest</c>).
/// </remarks>
public abstract class LinkingRequest
{
    /// <summary>
    /// Whether the request is for unlinking only (i.e. clearing the existing link).
    /// </summary>
    public bool IsUnlink { get; protected set; }
}