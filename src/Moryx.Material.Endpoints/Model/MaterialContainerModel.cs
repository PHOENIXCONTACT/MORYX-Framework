// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Material.Endpoints.Model;

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

    [DataMember]
    public required MaterialContainerTypeModel Type { get; set; }

    [DataMember]
    public List<ReferenceModel> References { get; set; } = [];
}
