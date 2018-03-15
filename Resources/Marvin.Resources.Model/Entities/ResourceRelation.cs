using System.ComponentModel.DataAnnotations.Schema;
using Marvin.Model;

namespace Marvin.Resources.Model
{
    /// <summary>
    /// There are no comments for Marvin.Resources.Model.ResourceEntity in the schema.
    /// </summary>
    [Table(nameof(ResourceRelation), Schema = ResourcesConstants.SchemaName)]
    public class ResourceRelation : EntityBase
    {
        /// <summary>
        /// There are no comments for RelationType in the schema.
        /// </summary>
        public virtual int RelationType { get; set; }

        /// <summary>
        /// There are no comments for RelationName in the schema.
        /// </summary>
        public virtual string RelationName { get; set; }

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
        /// There are no comments for Source in the schema.
        /// </summary>
        public virtual ResourceEntity Source { get; set; }

        /// <summary>
        /// There are no comments for Target in the schema.
        /// </summary>
        public virtual ResourceEntity Target { get; set; }

        #endregion
    }
}
