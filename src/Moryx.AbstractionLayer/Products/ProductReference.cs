// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Identity;

namespace Moryx.AbstractionLayer.Products
{
    /// <summary>
    /// Temporary product reference to be replaced by the product management
    /// </summary>
    public class ProductReference : IProductType
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

        /// <summary>
        /// Unique id of this product
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Display name of this product
        /// </summary>
        public string Name { get; set; }

        /// <inheritdoc />
        public ProductState State { get; set; }

        /// <summary>
        /// Identity of this product
        /// </summary>
        public IIdentity Identity { get; set; }

        /// <summary>
        /// Create instance of this type
        /// </summary>
        public ProductInstance CreateInstance()
        {
            throw new InvalidOperationException("Reference products can not be instantiated. Please replace with a real product first!");
        }
    }
}
