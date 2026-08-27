// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Products;

/// <inheritdoc/>
public class ProductLinkRequestEventArgs(ProductLinkingRequest request, Func<LinkingResponse, Task> responseCallback) :
    LinkingRequestEventArgs(request, responseCallback)
{
    /// <summary>
    /// Strongly typed access to the product-specific request payload.
    /// </summary>
    public ProductLinkingRequest ProductRequest => (ProductLinkingRequest)Request;
}