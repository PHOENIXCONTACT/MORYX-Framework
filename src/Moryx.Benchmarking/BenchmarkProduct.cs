// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;

namespace Moryx.Benchmarking
{
    public class BenchmarkProduct : ProductType
    {
        /// <summary>
        /// Instantiate this product
        /// </summary>
        /// <returns>
        /// New instance
        /// </returns>
        protected override ProductInstance Instantiate()
        {
            return new BenchmarkInstance();
        }
    }
}
