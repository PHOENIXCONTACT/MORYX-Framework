// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Material.Endpoints;

[DataContract]
public enum MaterialStateClassificationModel
{
    [EnumMember]
    Uninitialized = 0,

    [EnumMember]
    Requested = 1,

    [EnumMember]
    Inbound = 2,

    [EnumMember]
    Available = 3,

    [EnumMember]
    Outbound = 4,

    [EnumMember]
    Deregistered = 5
}

[DataContract]
public class ContainerHostModel
{
    [DataMember]
    public long Id { get; set; }

    [DataMember]
    public string Name { get; set; } = string.Empty;

    [DataMember]
    public string? Description { get; set; }
}

[DataContract]
public class MaterialContainerModel
{
    [DataMember]
    public long Id { get; set; }

    [DataMember]
    public string Name { get; set; } = string.Empty;

    [DataMember]
    public ContainerHostModel? ContainerHost { get; set; }

    [DataMember]
    public string? Identity { get; set; }

    [DataMember]
    public string? Material { get; set; }

    [DataMember]
    public double Quantity { get; set; }

    [DataMember]
    public string? Unit { get; set; }

    [DataMember]
    public MaterialStateClassificationModel State { get; set; }
}