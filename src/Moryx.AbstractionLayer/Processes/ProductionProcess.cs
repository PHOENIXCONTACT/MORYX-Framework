// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;

namespace Moryx.AbstractionLayer.Processes
{
    /// <summary>
    /// Concrete process of producing a single product instance
    /// </summary>
    public class ProductionProcess : Process
    {
        /// <summary>
        /// The product instance produced by this process.
        /// </summary>
        public ProductInstance ProductInstance { get; set; }
    }
}
