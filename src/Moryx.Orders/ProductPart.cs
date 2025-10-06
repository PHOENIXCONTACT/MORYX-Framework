// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Products;

namespace Moryx.Orders
{
    /// <summary>
    /// Business object for the product parts of an operation
    /// </summary>
    public class ProductPart : IProductType
    {
        /// <summary>
        /// Id of this part
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Typed product identity
        /// </summary>
        public ProductIdentity Identity { get; set; }

        /// <inheritdoc />
        IIdentity IIdentifiableObject.Identity
        {
            get => Identity;
            set
            {
                if (value is ProductIdentity prodIdentity)
                    Identity = prodIdentity;
            }
        }

        /// <summary>
        /// Name of the part like the product name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// State of the product
        /// </summary>
        public ProductState State { get; set; }

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
        public ProductInstance CreateInstance()
        {
            throw new NotSupportedException("Creating an instance of this product is not supported.");
        }
    }
}
