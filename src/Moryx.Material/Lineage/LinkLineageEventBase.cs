// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Material.Lineage;

/// <summary>
/// Base class for lineage events that record a (un)linking action between a container
/// and a domain object.
/// </summary>
/// <remarks>
/// Domain-specific integrations (e.g. order linking) should subclass this and add their
/// specific reference data.
/// </remarks>
[DataContract]
public abstract class LinkLineageEventBase : LineageEventBase
{
    /// <summary>
    /// Indicates whether this is a successful link / unlink, or a failed attempt.
    /// </summary>
    [DataMember]
    public bool Successful { get; set; }

    /// <summary>
    /// Human readable summary of the linking action.
    /// </summary>
    [DataMember]
    public string? Description { get; set; }
}