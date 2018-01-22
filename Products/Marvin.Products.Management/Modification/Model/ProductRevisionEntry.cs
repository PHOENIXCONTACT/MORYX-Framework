using System;
using System.Runtime.Serialization;
using Marvin.AbstractionLayer;

namespace Marvin.Products.Management.Modification
{
    /// <summary>
    /// Entity that represents a product revision.
    /// </summary>
    [DataContract]
    public class ProductRevisionEntry
    {
        /// <summary>
        /// The id if the product.
        /// </summary>
        [DataMember]
        public long ProductId { get; set; }

        /// <summary>
        /// The revision of the product.
        /// </summary>
        [DataMember]
        public short Revision { get; set; }

        /// <summary>
        /// State of the product. <see cref="ProductState"/>
        /// </summary>
        [DataMember]
        public ProductState State { get; set; }

        /// <summary>
        /// Time when the revision was created.
        /// </summary>
        [DataMember]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Time when the revision was released.
        /// </summary>
        [DataMember]
        public DateTime? ReleaseDate { get; set; }

        /// <summary>
        /// A comment to the revision and/or release.
        /// </summary>
        [DataMember]
        public string Comment { get; set; }
    }
}