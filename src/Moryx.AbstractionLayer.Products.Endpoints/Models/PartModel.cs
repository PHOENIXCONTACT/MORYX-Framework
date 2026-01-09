// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Serialization;

namespace Moryx.AbstractionLayer.Products.Endpoints.Models;

[DataContract]
public class PartModel
{
    [DataMember]
    public long Id { get; set; }

    [DataMember]
    public Entry Properties { get; set; }

    [DataMember]
    public ProductModel Product { get; set; }
}