// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Workplans;

namespace Moryx.AbstractionLayer.Products.Endpoints.Models;

[DataContract]
public class WorkplanModel
{
    [DataMember]
    public long Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public int Version { get; set; }

    [DataMember]
    public WorkplanState State { get; set; }
}