// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Text.Json.Serialization;

namespace Moryx.Material.Endpoints.Model;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(OrderReferenceModel), nameof(OrderReferenceModel))]
public class ReferenceModel
{
    public string? DisplayName { get; set; }
}