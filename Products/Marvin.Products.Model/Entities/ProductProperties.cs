using System.ComponentModel.DataAnnotations.Schema;
using Marvin.Model;

namespace Marvin.Products.Model
{
    /// <summary>
    /// There are no comments for Marvin.Products.Model.ProductProperties in the schema.
    /// </summary>
    [Table(nameof(ProductProperties), Schema = ProductsConstants.SchemaName)]
    public class ProductProperties : ModificationTrackedEntityBase
    {
        /// <summary>
        /// There are no comments for State in the schema.
        /// </summary>
        public virtual int State { get; set; }

        /// <summary>
        /// There are no comments for Name in the schema.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// There are no comments for ProductId in the schema.
        /// </summary>
        public virtual long? ProductId { get; set; }

        #region Navigation Properties

        /// <summary>
        /// Parts
        /// </summary>
        public virtual ProductEntity Product { get; set; }

        #endregion
    }
}
