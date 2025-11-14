// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Identity;

namespace Moryx.AbstractionLayer.Products
{
    /// <summary>
    /// Temporary product reference to be replaced by the product management
    /// </summary>
    public class ProductReference : ProductType
    {
        /// <summary>
        /// Create a reference product by giving an id
        /// </summary>
        public ProductReference(long id)
        {
            Id = id;
        }

        /// <summary>
        /// Create a reference product by giving an identity
        /// </summary>
        /// <param name="identity">Identity information of this ProductReference</param>
        public ProductReference(IIdentity identity)
        {
            Identity = identity;
        }

        /// <inheritdoc />
        protected override ProductInstance Instantiate()
        {
            throw new InvalidOperationException("Reference products can not be instantiated. Please replace with a real product first!");
        }
    }
}
