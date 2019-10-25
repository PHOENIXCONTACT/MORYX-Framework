using System.ComponentModel;
using Marvin.AbstractionLayer;

namespace Marvin.Products.Samples
{
    public class NeedlePartLink : ProductPartLink<NeedleType>
    {
        public NeedlePartLink()
        {
        }

        public NeedlePartLink(long id) : base(id)
        {
        }

        [Description("Which role does the needle have")]
        public NeedleRole Role { get; set; }

        public override ProductInstance Instantiate()
        {
            var needle = (NeedleInstance) base.Instantiate();
            needle.Role = Role;
            return needle;
        }
    }
}