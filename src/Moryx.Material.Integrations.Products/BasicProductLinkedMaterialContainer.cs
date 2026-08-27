// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;

namespace Moryx.Material.Integrations.Products;

/// <summary>
/// Ready-to-use, non-abstract <see cref="ProductLinkedMaterialContainer"/> for the most common cases.
/// </summary>
[DisplayName("Basic Product-Linked Material Container")]
[Description("Default product-linkable container resource.")]
public class BasicProductLinkedMaterialContainer : ProductLinkedMaterialContainer
{
}