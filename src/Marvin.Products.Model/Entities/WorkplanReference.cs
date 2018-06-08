using System.ComponentModel.DataAnnotations.Schema;
using Marvin.Model;

namespace Marvin.Products.Model
{
    public class WorkplanReference : EntityBase
    {
        public virtual int ReferenceType { get; set; }

        public virtual long SourceId { get; set; }

        public virtual long TargetId { get; set; }

        public virtual WorkplanEntity Target { get; set; }

        public virtual WorkplanEntity Source { get; set; }
    }
}
