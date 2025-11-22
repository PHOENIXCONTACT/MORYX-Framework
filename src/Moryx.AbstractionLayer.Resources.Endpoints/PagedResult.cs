// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Resources.Endpoints;

/// <summary>
/// Generic paged result returned from endpoints providing the <see cref="Items"/>
/// with corresponding metadata from the request.
/// </summary>
/// <typeparam name="ItemType">Type of the items to be returned</typeparam>
public sealed class PagedResult<ItemType>
{
    /// <summary>
    /// The requested page from the total set of items
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// The maximum page size provided for the request
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// The total number of items available
    /// </summary>
    public long TotalCount { get; set; }

    /// <summary>
    /// The items returned for the request
    /// </summary>
    public ItemType[] Items { get; set; }
}
