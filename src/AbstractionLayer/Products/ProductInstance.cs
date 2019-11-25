using System.Runtime.Serialization;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Base class for all product instances.
    /// </summary>
    [DataContract]
    public abstract class ProductInstance : IPersistentObject
    {
        /// <summary>
        /// The ID of this article
        /// </summary>
        public long Id { get; set; }

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