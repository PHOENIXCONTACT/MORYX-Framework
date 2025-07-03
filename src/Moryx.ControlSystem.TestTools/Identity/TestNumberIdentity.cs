using Moryx.AbstractionLayer.Identity;

namespace Moryx.ControlSystem.TestTools.Identity
{
    public class TestNumberIdentity : NumberIdentity
    {
        public TestNumberIdentity(int numberType) : base(numberType)
        {
        }

        public TestNumberIdentity(int numberType, string identifier) : base(numberType, identifier)
        {
        }
    }
}