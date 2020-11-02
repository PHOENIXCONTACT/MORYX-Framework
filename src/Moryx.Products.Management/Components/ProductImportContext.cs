// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Result of a product import
    /// </summary>
    public class ProductImportContext
    {
        /// <summary>
        /// Session guid of the import
        /// </summary>
        public Guid Session { get; } = Guid.NewGuid();
    }
}