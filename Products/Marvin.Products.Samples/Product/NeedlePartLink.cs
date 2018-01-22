using Marvin.AbstractionLayer;

namespace Marvin.Products.Samples
{
    public class NeedlePartLink : ProductPartLink<NeedleProduct>
    {
        public NeedlePartLink()
        {
        }

        public NeedlePartLink(long id) : base(id)
        {
        }

        public NeedleRole Role { get; set; }

        public override Article Instantiate()
        {
            var needle = (NeedleArticle) base.Instantiate();
            needle.Role = Role;
            return needle;
        }
    }
}