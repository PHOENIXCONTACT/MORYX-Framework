// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Serialization;

namespace Moryx.AbstractionLayer.Products.Endpoints.Models;

[DataContract]
public class ProductInstanceModel
{
    [DataMember]
    public long Id { get; set; }

    /// <summary>
    /// The name of the product type of this instance
    /// </summary>
    [DataMember]
    public string Type { get; set; }

    /// <summary>
    /// The current state of the instance
    /// </summary>
    [DataMember]
    public ProductInstanceState State { get; set; }

    [DataMember]
    public Entry Properties { get; set; }
}