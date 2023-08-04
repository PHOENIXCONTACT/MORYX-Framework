using Moryx.AbstractionLayer.Capabilities;
using System;

namespace Moryx.AbstractionLayer.Tests.TestData
{
    [ActivityResults(typeof(TestResults))]
    public class TestActivity : Activity<NullActivityParameters>
    {
        public override ProcessRequirement ProcessRequirement => throw new NotImplementedException();

        public override ICapabilities RequiredCapabilities => throw new NotImplementedException();

        protected override ActivityResult CreateFailureResult()
        {
            throw new NotImplementedException();
        }

        protected override ActivityResult CreateResult(long resultNumber)
        {
            throw new NotImplementedException();
        }
    }
}
