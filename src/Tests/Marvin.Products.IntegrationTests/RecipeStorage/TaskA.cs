using Marvin.AbstractionLayer;
using Marvin.AbstractionLayer.Capabilities;
using Marvin.Serialization;

namespace Marvin.Products.IntegrationTests
{
    public class TaskA : TaskStep<ActivityA, ParametersA>
    {
        public override string Name => nameof(TaskA);
    }

    public class ParametersA : ParametersBase
    {
        [EditorVisible]
        public int Foo { get; set; }

        protected override ParametersBase ResolveBinding(IProcess process)
        {
            return this;
        }
    }

    [ActivityResults(typeof(DefaultActivityResult))]
    public class ActivityA : Activity<ParametersA>
    {
        public override string Type => nameof(ActivityA);

        public override ProcessRequirement ProcessRequirement => ProcessRequirement.NotRequired;

        public override ICapabilities RequiredCapabilities { get; }


        protected override ActivityResult CreateResult(long resultNumber)
        {
            throw new System.NotImplementedException();
        }

        protected override ActivityResult CreateFailureResult()
        {
            throw new System.NotImplementedException();
        }
    }
}
