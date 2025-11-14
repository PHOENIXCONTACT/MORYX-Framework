// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Resources.Endpoints;

/// <summary>
/// Extension methods on the <see cref="PagedResult{ItemType}"/> for fluent instance creation
/// </summary>
public static class PagedResultExtensions
{
    extension<ItemType>(PagedResult<ItemType> pagedResult)
    {
        /// <summary>
        /// Adds the <paramref name="pagination"/> meta informtaion to this <paramref name="pagedResult"/>
        /// </summary>
        /// <returns></returns>
        public PagedResult<ItemType> With(PaginationParameters pagination)
        {
            pagedResult.PageNumber = pagination.PageNumber;
            pagedResult.PageSize = pagination.PageSize;
            // ToDo: Verify that already added results match the metadata
            return pagedResult;
        }

        /// <summary>
        /// Populates this <paramref name="pagedResult"/> with the <paramref name="fullSet"/>
        /// of items taking already configured <see cref="PagedResult{ItemType}.PageNumber"/>
        /// and <see cref="PagedResult{ItemType}.PageSize"/> into account
        /// </summary>
        /// <returns></returns>
        public PagedResult<ItemType> Of(IEnumerable<ItemType> fullSet)
        {
            // ToDo: Account for unset metadata
            pagedResult.TotalCount = fullSet.Count();
            var skip = (pagedResult.PageNumber - 1) * pagedResult.PageSize;
            pagedResult.Items = [.. fullSet.Skip(skip).Take(pagedResult.PageSize)];
            return pagedResult;
        }
    }
}
