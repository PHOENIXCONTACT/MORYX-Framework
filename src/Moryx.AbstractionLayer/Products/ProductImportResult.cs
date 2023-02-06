// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;

namespace Moryx.AbstractionLayer.Products
{
    /// <summary>
    /// Result of the product import
    /// </summary>
    public class ProductImportResult
    {
        /// <summary>
        /// Types that were imported
        /// </summary>
        public IReadOnlyList<ProductType> ImportedTypes { get; set; } = new List<ProductType>();
    }
}