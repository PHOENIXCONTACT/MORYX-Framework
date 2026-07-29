// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.Material.Integrations.Orders;

/// <summary>
/// Ready-to-use, non-abstract <see cref="OrderLinkedMaterialContainer"/> for the most common cases.
/// </summary>
[DisplayName("Basic Order-Linked Material Container")]
[Description("Default order-linkable container resource.")]
public class BasicOrderLinkedMaterialContainer : OrderLinkedMaterialContainer
{
}
