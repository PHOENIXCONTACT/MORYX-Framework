// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Orders.Integrator.Components;

// ToDo: Clean up: Make used information directly accasible and drop the rest
internal class RequestContext(IOrderLinkedMaterialContainer c, OrderLinkRequestEventArgs e)
{
    public IOrderLinkedMaterialContainer Container { get; } = c;

    public OrderLinkingRequest OrderRequest { get; } = e.OrderRequest;

    public Func<LinkingResponse, Task> ResponseCallback { get; } = e.ResponseCallback;
}
