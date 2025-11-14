// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Resources.Endpoints;

/// <summary>
/// Parameters used for paging requests.
/// </summary>
public sealed class PaginationParameters
{
    /// <summary>
    /// The page number for a query returning a list of items; default to 1
    /// </summary>
    public int PageNumber { get; set => field = Math.Max(value, 1); } = 1;

    /// <summary>
    /// The number of items provided on the page; defaults to 20 with a minimum of 0 and a maximum of 100
    /// </summary>
    public int PageSize { get; set => field = Math.Min(Math.Max(value, 0), 100); } = 20;
}
