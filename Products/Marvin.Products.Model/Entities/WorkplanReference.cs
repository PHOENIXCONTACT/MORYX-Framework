using System.ComponentModel.DataAnnotations.Schema;
using Marvin.Model;

namespace Marvin.Products.Model
{
    /// <summary>
    /// There are no comments for Marvin.Products.Model.WorkplanReference in the schema.
    /// </summary>
    [Table(nameof(WorkplanReference), Schema = ProductsConstants.SchemaName)]
    public class WorkplanReference : EntityBase
    {
        /// <summary>
        /// There are no comments for ReferenceType in the schema.
        /// </summary>
        public virtual int ReferenceType { get; set; }

        /// <summary>
        /// There are no comments for SourceId in the schema.
        /// </summary>
        public virtual long SourceId { get; set; }

        /// <summary>
        /// There are no comments for TargetId in the schema.
        /// </summary>
        public virtual long TargetId { get; set; }

        #region Navigation Properties

        /// <summary>
        /// There are no comments for Target in the schema.
        /// </summary>
        public virtual WorkplanEntity Target { get; set; }

        /// <summary>
        /// There are no comments for Source in the schema.
        /// </summary>
        public virtual WorkplanEntity Source { get; set; }

        #endregion
    }
}
