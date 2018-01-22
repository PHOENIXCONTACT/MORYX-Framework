using Marvin.AbstractionLayer;

namespace Marvin.Products.Samples
{
    public class NeedleArticle : Article<NeedleProduct>
    {
        public override string Type => nameof(NeedleArticle);

        public NeedleRole Role { get; set; }
    }
}