using Marvin.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

// ReSharper disable once CheckNamespace
namespace Marvin.Resources.Model
{
    public class ResourceEntity : ModificationTrackedEntityBase
    {
        [Index]
        public virtual string Name { get; set; }

        /// <summary>
        /// Identifier used within this system - e.g. the COM-Number within the machine.
        /// </summary>
        [Index]
        public virtual string LocalIdentifier { get; set; }

        [Index]
        public virtual string GlobalIdentifier { get; set; }

        public virtual string Description { get; set; }

        public virtual string ExtensionData { get; set; }

        public virtual string Type { get; set; }

        public virtual ICollection<ResourceRelation> Targets { get; set; }

        public virtual ICollection<ResourceRelation> Sources { get; set; }
    }
}
