// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Products
{
    /// <summary>
    /// Base class that allows to assign a value to <see cref="IPersistentObject.Id"/>
    /// </summary>
    public abstract class ProductPartLink : IProductPartLink
    {
        /// <summary>
        /// Default constructor for a new part link
        /// </summary>
        internal ProductPartLink()
        {
        }

        /// <summary>
        /// Constructor for parts links that are read from database
        /// </summary>
        /// <param name="id"></param>
        internal ProductPartLink(long id)
        {
            Id = id;
        }

        /// <summary>
        /// Unique id of this link object
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Parent product for this part link
        /// </summary>
        public IProductType Parent { get; set; }

        /// <summary>
        /// Generic product reference of this link
        /// </summary>
        public IProductType Product { get; set; }

        /// <summary>
        /// Create single instance for this part
        /// </summary>
        public virtual ProductInstance Instantiate()
        {
            var instance = Product.CreateInstance();
            instance.PartLink = this;
            return instance;
        }
    }

    /// <summary>
    /// Class to create generic part structure
    /// </summary>
    public class ProductPartLink<TProduct> : ProductPartLink, IProductPartLink<TProduct>
        where TProduct : class, IProductType
    {
        /// <summary>
        /// Default constructor for a new part link
        /// </summary>
        public ProductPartLink()
        {
        }

        /// <summary>
        /// Constructor for parts links that are read from database
        /// </summary>
        /// <param name="id"></param>
        public ProductPartLink(long id) : base(id)
        {
        }

        /// <summary>
        /// Reference to the generic product
        /// </summary>
        public new TProduct Product
        {
            get => (TProduct)base.Product;
            set => base.Product = value;
        }
    }
}
