// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Products
{
    /// <summary>
    /// Context of a product import session
    /// </summary>
    public class ProductImportContext
    {
        /// <summary>
        /// Session guid of the import
        /// </summary>
        public Guid Session { get; } = Guid.NewGuid();
    }
}