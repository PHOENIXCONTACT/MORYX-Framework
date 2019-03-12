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

        public virtual string Description { get; set; }

        public virtual string ExtensionData { get; set; }

        public virtual string Type { get; set; }

        public virtual ICollection<ResourceRelation> Targets { get; set; }

        public virtual ICollection<ResourceRelation> Sources { get; set; }
    }
}
