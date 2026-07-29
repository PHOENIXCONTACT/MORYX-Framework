// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Orders.Integrator.Components;

// ToDo: Clean up: Make used information directly accasible and drop the rest
internal class AppliedContext(IOrderLinkedMaterialContainer c, OrderLinkAppliedEventArgs e)
{
    public IOrderLinkedMaterialContainer Container { get; } = c;

    public OrderLinkingRequest OrderRequest { get; } = e.OrderRequest;

    public ValidationContext Validation { get; } = e.Context;

    public OrderReference? OrderReference { get; } = e.AppliedReference;
}
