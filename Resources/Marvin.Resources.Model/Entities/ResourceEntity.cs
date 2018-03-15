using Marvin.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marvin.Resources.Model
{

    /// <summary>
    /// There are no comments for Marvin.Resources.Model.ResourceEntity in the schema.
    /// </summary>
    [Table(nameof(ResourceEntity), Schema = ResourcesConstants.SchemaName)]
    public class ResourceEntity : ModificationTrackedEntityBase
    {
        /// <summary>
        /// There are no comments for Name in the schema.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Identifier used within this system - e.g. the COM-Number within the machine.
        /// </summary>
        public virtual string LocalIdentifier { get; set; }

        /// <summary>
        /// There are no comments for GlobalIdentifier in the schema.
        /// </summary>
        public virtual string GlobalIdentifier { get; set; }

        /// <summary>
        /// There are no comments for ExtensionData in the schema.
        /// </summary>
        public virtual string ExtensionData { get; set; }

        /// <summary>
        /// There are no comments for Type in the schema.
        /// </summary>
        public virtual string Type { get; set; }

        #region Navigation Properties

        /// <summary>
        /// There are no comments for Targets in the schema.
        /// </summary>
        public virtual ICollection<ResourceRelation> Targets { get; set; }

        /// <summary>
        /// There are no comments for Sources in the schema.
        /// </summary>
        public virtual ICollection<ResourceRelation> Sources { get; set; }

        #endregion
    }
}
