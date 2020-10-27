// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Identity;

namespace Moryx.AbstractionLayer.Products
{
    /// <summary>
    /// Base class for all product instances.
    /// </summary>
    [DataContract]
    public abstract class ProductInstance : IProductInstance, IPersistentObject
    {
        /// <summary>
        /// The Id of this instance
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The product type of this instance
        /// </summary>
        public IProductType Type { get; set; }

        /// <summary>
        /// The current state of the instance
        /// </summary>
        public ProductInstanceState State { get; set; }

        /// <summary>
        /// Part link that created this <see cref="ProductInstance"/>. This is <value>null</value> for root instances
        /// </summary>
        public IProductPartLink PartLink { get; set; }
    }

    /// <summary>
    /// Generic base class for product type access
    /// </summary>
    public abstract class ProductInstance<TProduct> : ProductInstance
        where TProduct : IProductType
    {
        /// <summary>
        /// Typed property for product access
        /// </summary>
        public new TProduct Type
        {
            get => (TProduct) base.Type;
            set => base.Type = value;
        }
    }
}
