using Marvin.Model;

namespace Marvin.Products.Model
{
    public class RevisionHistory : EntityBase
    {
        public virtual string Comment { get; set; }

        public virtual long ProductRevisionId { get; set; }

        public virtual ProductEntity ProductRevision { get; set; }
    }
}
