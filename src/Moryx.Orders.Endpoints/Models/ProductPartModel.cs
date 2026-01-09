// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Orders.Endpoints.Models;

[DataContract]
public class ProductPartModel
{
    [DataMember]
    public long Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Identifier { get; set; }

    [DataMember]
    public double Quantity { get; set; }

    [DataMember]
    public string Unit { get; set; }

    [DataMember]
    public StagingIndicator StagingIndicator { get; set; }

    [DataMember]
    public PartClassification Classification { get; set; }
}