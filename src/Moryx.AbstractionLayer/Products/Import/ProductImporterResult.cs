// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Products
{
    /// <summary>
    /// Return value for <see cref="IProductImporter"/>
    /// </summary>
    public class ProductImporterResult : ProductImportResult
    {
        /// <summary>
        /// Flag if all objects were saved
        /// </summary>
        public bool Saved { get; set; }
    }
}