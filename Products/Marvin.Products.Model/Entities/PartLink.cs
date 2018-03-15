using Marvin.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marvin.Products.Model
{
    /// <summary>
    /// There are no comments for Marvin.Products.Model.PartLink in the schema.
    /// </summary>
    [Table(nameof(PartLink), Schema = ProductsConstants.SchemaName)]
    public class PartLink : EntityBase
    {
        /// <summary>
        /// There are no comments for ParentId in the schema.
        /// </summary>
        public virtual long ParentId { get; set; }

        /// <summary>
        /// There are no comments for ChildId in the schema.
        /// </summary>
        public virtual long ChildId { get; set; }

        /// <summary>
        /// There are no comments for PropertyName in the schema.
        /// </summary>
        public virtual string PropertyName { get; set; }

        #region Navigation Properties

        /// <summary>
        /// There are no comments for Parent in the schema.
        /// </summary>
        public virtual ProductEntity Parent { get; set; }

        /// <summary>
        /// There are no comments for Child in the schema.
        /// </summary>
        public virtual ProductEntity Child { get; set; }

        #endregion
    }

}
