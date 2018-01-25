using Marvin.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marvin.Products.Model
{
    /// <summary>
    /// There are no comments for Marvin.Products.Model.RevisionHistory in the schema.
    /// </summary>
    [Table(nameof(RevisionHistory), Schema = ProductsConstants.SchemaName)]
    public class RevisionHistory : EntityBase
    {
        /// <summary>
        /// There are no comments for Comment in the schema.
        /// </summary>
        public virtual string Comment { get; set; }

        /// <summary>
        /// There are no comments for ProductRevisionId in the schema.
        /// </summary>
        public virtual long ProductRevisionId { get; set; }

        #region Navigation Properties

        /// <summary>
        /// There are no comments for ProductRevision in the schema.
        /// </summary>
        public virtual ProductEntity ProductRevision { get; set; }

        #endregion
    }
}
