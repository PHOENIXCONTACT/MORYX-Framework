// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Marvin.AbstractionLayer.Identity;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Base class for all product instances.
    /// </summary>
    [DataContract]
    public abstract class ProductInstance : IPersistentObject, IIdentifiableObject
    {
        /// <summary>
        /// The Id of this instance
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Generic identity of this instance
        /// </summary>
        public IIdentity Identity { get; set; }

        /// <summary>
        /// The current state of the instance
        /// </summary>
        public ProductInstanceState State { get; set; }

        /// <summary>
        /// Product this article is an instance of
        /// </summary>
        public IProductType ProductType { get; set; }

        /// <summary>
        /// Part link that created this <see cref="ProductInstance"/>. This is <value>null</value> for root instances
        /// </summary>
        public IProductPartLink PartLink { get; set; }
    }

    /// <summary>
    /// Generic base class for product access
    /// </summary>
    public abstract class ProductInstance<TProduct> : ProductInstance
        where TProduct : IProductType
    {
        /// <summary>
        /// Typed property for product access
        /// </summary>
        public new TProduct ProductType
        {
            get => (TProduct) base.ProductType;
            set => base.ProductType = value;
        }
    }
}
