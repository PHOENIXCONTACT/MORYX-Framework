// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.Products.Management.Importers;

namespace Moryx.Products.Management
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