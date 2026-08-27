// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Products.Integrator.Components;

// ToDo: Clean up: Make used information directly accessible and drop the rest
internal class AppliedContext(IProductLinkedMaterialContainer c, ProductLinkAppliedEventArgs e)
{
    public IProductLinkedMaterialContainer Container { get; } = c;

    public ProductLinkingRequest ProductRequest { get; } = e.ProductRequest;

    public ValidationContext Validation { get; } = e.Context;

    public ProductTypeReference? ProductReference { get; } = e.AppliedReference;
}