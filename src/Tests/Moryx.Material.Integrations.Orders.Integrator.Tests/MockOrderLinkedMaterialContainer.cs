// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Material.Integrations.Products;

namespace Moryx.Material.Integrations.Orders.Integrator.Tests;

internal sealed class MockOrderLinkedMaterialContainer : OrderLinkedMaterialContainer
{
}

/// <summary>
/// Container linked to an order and a product type without deriving from either convenience base class.
/// </summary>
[DataContract]
internal sealed class MockOrderAndProductLinkedMaterialContainer : MaterialContainer,
    IOrderLinkedMaterialContainer, IProductLinkedMaterialContainer
{
    [DataMember]
    public OrderLink OrderLink { get; } = new();

    [DataMember]
    public ProductLink ProductLink { get; } = new();
}
