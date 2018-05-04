using Marvin.Model;

// ReSharper disable once CheckNamespace
namespace Marvin.Resources.Model
{
    public class ResourceRelation : EntityBase
    {
        public virtual int RelationType { get; set; }

        public virtual string SourceName { get; set; }

        public virtual long SourceId { get; set; }

        public virtual string TargetName { get; set; }

        public virtual long TargetId { get; set; }

        public virtual ResourceEntity Source { get; set; }

        public virtual ResourceEntity Target { get; set; }
    }
}