using System.ComponentModel;
using Marvin.AbstractionLayer;

namespace Marvin.Products.Samples
{
    [DisplayName("Watch Needle")]
    public class NeedleType : ProductType
    {
        [Description("Length of the needle")]
        public int Length { get; set; }

        protected override ProductInstance Instantiate()
        {
            return new NeedleInstance();
        }
    }
}