using Marvin.Model;

namespace Marvin.Products.Model
{
    public class ProductRecipeEntity : ModificationTrackedEntityBase
    {
        public virtual string Name { get; set; }

        public virtual int Revision { get; set; }

        public virtual int Classification { get; set; }

        public virtual int State { get; set; }

        public virtual long WorkplanId { get; set; }

        public virtual long ProductId { get; set; }

        public virtual ProductEntity Product { get; set; }

        public virtual WorkplanEntity Workplan { get; set; }
    }
}
