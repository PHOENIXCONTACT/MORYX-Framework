// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Material.Endpoints.Model;

[DataContract]
public class ContainerHostModel
{
    [DataMember]
    public long Id { get; set; }

    [DataMember]
    public string Name { get; set; } = string.Empty;

    [DataMember]
    public string? TypeName { get; set; }

    [DataMember]
    public string? TypeDescription { get; set; }
}
