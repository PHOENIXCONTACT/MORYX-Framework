using System.ComponentModel;
using Marvin.AbstractionLayer;

namespace Marvin.Products.Samples
{
    [DisplayName("Uhrzeiger")]
    public class NeedleProduct : Product
    {
        public override string Type => nameof(NeedleProduct);

        [DisplayName("Länge")]
        [Description("Länge des Zeigers")]
        public int Length { get; set; }

        protected override Article Instantiate()
        {
            return new NeedleArticle();
        }
    }
}