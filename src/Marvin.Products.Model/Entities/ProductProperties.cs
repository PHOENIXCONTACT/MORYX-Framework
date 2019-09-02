using Marvin.Model;

namespace Marvin.Products.Model
{
    public class ProductProperties : ModificationTrackedEntityBase
    {
        public virtual string Name { get; set; }

        public virtual long? ProductId { get; set; }

        public virtual ProductEntity Product { get; set; }
    }
}
