using System.ComponentModel.DataAnnotations.Schema;
using Marvin.Model;

namespace Marvin.Products.Model
{

    /// <summary>
    /// There are no comments for Marvin.Products.Model.ProductRecipeEntity in the schema.
    /// </summary>
    [Table(nameof(ProductRecipeEntity), Schema = ProductsConstants.SchemaName)]
    public class ProductRecipeEntity : ModificationTrackedEntityBase
    {
        /// <summary>
        /// There are no comments for Name in the schema.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// There are no comments for Revision in the schema.
        /// </summary>
        public virtual int Revision { get; set; }

        /// <summary>
        /// There are no comments for Classification in the schema.
        /// </summary>
        public virtual int Classification { get; set; }

        /// <summary>
        /// There are no comments for State in the schema.
        /// </summary>
        public virtual int State { get; set; }

        /// <summary>
        /// There are no comments for WorkplanId in the schema.
        /// </summary>
        public virtual long WorkplanId { get; set; }

        /// <summary>
        /// There are no comments for ProductId in the schema.
        /// </summary>
        public virtual long ProductId { get; set; }

        #region Navigation Properties

        /// <summary>
        /// There are no comments for Product in the schema.
        /// </summary>
        public virtual ProductEntity Product { get; set; }

        /// <summary>
        /// There are no comments for Workplan in the schema.
        /// </summary>
        public virtual WorkplanEntity Workplan { get; set; }

        #endregion
    }
}
