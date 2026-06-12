// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Material.Lineage;

/// <summary>
/// Default base class for <see cref="ILineageEvent"/> implementations.
/// </summary>
[DataContract]
public abstract class LineageEventBase : ILineageEvent
{
    /// <inheritdoc />
    [DataMember]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <inheritdoc />
    [DataMember]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <inheritdoc />
    [DataMember]
    public long ContainerId { get; set; }
}