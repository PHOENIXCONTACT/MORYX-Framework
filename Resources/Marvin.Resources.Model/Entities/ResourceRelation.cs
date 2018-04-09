using Marvin.Model;

namespace Marvin.Resources.Model
{
    public class ResourceRelation : EntityBase
    {
        public virtual int RelationType { get; set; }

        public virtual string RelationName { get; set; }

        public virtual long SourceId { get; set; }

        public virtual long TargetId { get; set; }

        public virtual ResourceEntity Source { get; set; }

        public virtual ResourceEntity Target { get; set; }
    }
}