// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.AbstractionLayer.Products;

namespace Marvin.AbstractionLayer.TestTools
{
    /// <summary>
    /// Dummy implementation of a <see cref="ProductType"/>
    /// </summary>
    public class DummyProductType : ProductType
    {
        /// <inheritdoc />
        protected override ProductInstance Instantiate()
        {
            return new DummyProductInstance();
        }
    }
}
