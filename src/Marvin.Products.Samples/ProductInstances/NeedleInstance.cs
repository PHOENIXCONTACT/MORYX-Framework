using Marvin.AbstractionLayer;

namespace Marvin.Products.Samples
{
    public class NeedleInstance : ProductInstance<NeedleType>
    {
        public override string Type => nameof(NeedleInstance);

        public NeedleRole Role { get; set; }
    }
}