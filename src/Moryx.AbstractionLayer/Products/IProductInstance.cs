// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Products
{
    /// <summary>
    /// Interface for product instances
    /// </summary>
    public interface IProductInstance
    {
        /// <summary>
        /// Id of the instance
        /// </summary>
        long Id { get; }

        /// <summary>
        /// The product type of this instance
        /// </summary>
        IProductType Type { get; }

        /// <summary>
        /// Part link that created this <see cref="IProductInstance"/>. This is <value>null</value> for root instances
        /// </summary>
        IProductPartLink PartLink { get; }

        /// <summary>
        /// The current state of the instance
        /// </summary>
        ProductInstanceState State { get; set; }
    }
}