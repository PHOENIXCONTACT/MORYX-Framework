using Marvin.Model;

namespace Marvin.Products.Model
{
    public class PartLink : EntityBase
    {
        public virtual long ParentId { get; set; }

        public virtual long ChildId { get; set; }

        public virtual string PropertyName { get; set; }

        public virtual ProductEntity Parent { get; set; }

        public virtual ProductEntity Child { get; set; }
    }

}
