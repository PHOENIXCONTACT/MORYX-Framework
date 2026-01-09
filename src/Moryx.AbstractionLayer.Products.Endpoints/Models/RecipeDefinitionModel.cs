// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.AbstractionLayer.Products.Endpoints.Models;

[DataContract]
public class RecipeDefinitionModel
{
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string DisplayName { get; set; }

    [DataMember]
    public bool HasWorkplans { get; set; }
}