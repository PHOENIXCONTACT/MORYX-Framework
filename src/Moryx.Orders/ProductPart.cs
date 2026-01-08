// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;

namespace Moryx.Orders
{
    /// <summary>
    /// Business object for the product parts of an operation
    /// </summary>
    public class ProductPart : ProductType
    {
        /// <summary>
        /// Identity of the product part of type <see cref="ProductIdentity"/>
        /// </summary>
        public new ProductIdentity Identity
        {
            get => (ProductIdentity)base.Identity;
            set
            {
                base.Identity = value;
            }
        }

        /// <summary>
        /// Quantity of this part which are necessary to produce one product
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Unit of the quantity
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Indicator about the staging of this product part
        /// </summary>
        public StagingIndicator StagingIndicator { get; set; }

        /// <summary>
        /// Classification of the part
        /// </summary>
        public PartClassification Classification { get; set; }

        /// <inheritdoc />
        protected override ProductInstance Instantiate()
        {
            throw new NotSupportedException("Creating an instance of this product is not supported.");
        }
    }
}
