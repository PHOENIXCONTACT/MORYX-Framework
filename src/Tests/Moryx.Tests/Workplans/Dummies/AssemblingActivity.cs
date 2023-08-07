using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.Tests.Workplans.Dummies
{
    [ActivityResults(typeof(DefaultActivityResult))]
    public class AssemblingActivity : Activity<AssemblingParameters>
    {
        public override ProcessRequirement ProcessRequirement => ProcessRequirement.NotRequired;

        public override ICapabilities RequiredCapabilities => new AssemblingCapabilities();

        protected override ActivityResult CreateResult(long resultNumber)
        {
            return ActivityResult.Create((DefaultActivityResult)resultNumber);
        }

        protected override ActivityResult CreateFailureResult()
        {
            return ActivityResult.Create(DefaultActivityResult.Failed);
        }
    }
}
