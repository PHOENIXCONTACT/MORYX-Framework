using Marvin.AbstractionLayer;

namespace Marvin.Products.Samples
{
    public class NeedleProduct : Product
    {
        public override string Type => nameof(NeedleProduct);

        public int Length { get; set; }

        protected override Article Instantiate()
        {
            return new NeedleArticle();
        }
    }
}