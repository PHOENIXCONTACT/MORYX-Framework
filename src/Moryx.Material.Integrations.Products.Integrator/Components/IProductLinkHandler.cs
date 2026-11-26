// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Products.Integrator.Components;

/// <summary>
/// Executes the linking protocol for all <see cref="ProductLink"/>s, orchestrating the configured
/// <see cref="ProductLinkingHook"/> plugins.
/// </summary>
internal interface IProductLinkHandler : ILinkHandler<ProductTypeReference>
{
}
