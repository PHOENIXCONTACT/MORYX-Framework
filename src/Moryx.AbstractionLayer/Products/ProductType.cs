// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Identity;

namespace Moryx.AbstractionLayer.Products
{
    /// <summary>
    /// Base class for all products
    /// </summary>
    public abstract class ProductType : IPersistentObject, IIdentifiableObject
    {
        /// <inheritdoc />
        public long Id { get; set; }

        /// <summary>
        /// Name of the product
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Current state of the products
        /// </summary>
        public ProductState State { get; set; }

        /// <inheritdoc />
        public IIdentity Identity { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Identity?.ToString();
        }

        /// <summary>
        /// Create instance of this type
        /// </summary>
        public ProductInstance CreateInstance()
        {
            var instance = Instantiate();
            instance.Type = this;
            return instance;
        }

        /// <summary>
        /// Instantiate this product
        /// </summary>
        /// <returns>New instance</returns>
        protected abstract ProductInstance Instantiate();
    }
}
