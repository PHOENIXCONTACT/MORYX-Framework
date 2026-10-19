// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Workplans.Web.Models;

/// <summary>
/// DTO type for workplan
/// </summary>
[DataContract]
public class WorkplanModel
{
    /// <inheritdoc cref="Workplan.Id" />
    [DataMember]
    public long Id { get; set; }

    /// <inheritdoc cref="Workplan.Name" />
    [DataMember]
    public string Name { get; set; }

    /// <inheritdoc cref="Workplan.Version" />
    [DataMember]
    public int Version { get; set; }

    /// <inheritdoc cref="Workplan.State" />
    [DataMember]
    public WorkplanState State { get; set; }
}
