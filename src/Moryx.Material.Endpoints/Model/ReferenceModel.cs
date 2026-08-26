// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Text.Json.Serialization;

namespace Moryx.Material.Endpoints.Model;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "ReferenceModel")]
[JsonDerivedType(typeof(OrderReferenceModel), "OrderReference")]
public class ReferenceModel
{
    public string? FullName { get; set; }

    public string? DisplayName { get; set; }
}
