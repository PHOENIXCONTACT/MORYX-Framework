using System.ComponentModel.DataAnnotations;
using Marvin.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marvin.Products.Model
{
    /// <summary>
    /// There are no comments for Marvin.Products.Model.ProductDocument in the schema.
    /// </summary>
    [Table(nameof(ProductDocument), Schema = ProductsConstants.SchemaName)]
    public class ProductDocument : ModificationTrackedEntityBase
    {
        /// <summary>
        /// There are no comments for FileName in the schema.
        /// </summary>
        [MaxLength(byte.MaxValue)]
        public virtual string FileName { get; set; }

        /// <summary>
        /// There are no comments for FilesystemPath in the schema.
        /// </summary>
        [MaxLength(byte.MaxValue)]
        public virtual string FilesystemPath { get; set; }

        /// <summary>
        /// There are no comments for File in the schema.
        /// </summary>
        public virtual byte[] File { get; set; }

        /// <summary>
        /// There are no comments for Version in the schema.
        /// </summary>
        public virtual int Version { get; set; }

        /// <summary>
        /// There are no comments for Hash in the schema.
        /// </summary>
        [MaxLength(ushort.MaxValue)]
        public virtual string Hash { get; set; }

        /// <summary>
        /// There are no comments for ProductId in the schema.
        /// </summary>
        public virtual long ProductId { get; set; }

        #region Navigation Properties

        /// <summary>
        /// There are no comments for Product in the schema.
        /// </summary>
        public virtual ProductEntity Product { get; set; }

        #endregion
    }
}
