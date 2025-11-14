// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Resources.Endpoints;

/// <summary>
/// Extension methods for handling pagination
/// </summary>
public static class PaginationParametersExtensions
{
    extension(PaginationParameters pagination)
    {
        /// <summary>
        /// Calculates the number of items to be skipped with the request
        /// </summary>
        public int Skip() => (pagination.PageNumber - 1) * pagination.PageSize;
    }
}
