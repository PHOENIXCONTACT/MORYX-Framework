// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Orders;

/// <inheritdoc/>
public class OrderLinkRequestEventArgs(OrderLinkingRequest request, Func<LinkingResponse, Task> responseCallback) :
    LinkingRequestEventArgs(request, responseCallback)
{
    /// <summary>
    /// Strongly typed access to the order-specific request payload.
    /// </summary>
    public OrderLinkingRequest OrderRequest => (OrderLinkingRequest)Request;
}
