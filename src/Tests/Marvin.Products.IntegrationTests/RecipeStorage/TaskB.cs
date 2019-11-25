using Marvin.AbstractionLayer;
using Marvin.AbstractionLayer.Capabilities;
using Marvin.Serialization;
using Marvin.Workflows;

namespace Marvin.Products.IntegrationTests
{
    public class TaskB : TaskStep<ActivityB, ParametersB>
    {
        public override string Name => nameof(TaskB);
    }

    public class ParametersB : ParametersBase
    {
        [EditorVisible]
        public SubParameter[] Subs { get; set; }

        protected override ParametersBase ResolveBinding(IProcess process)
        {
            return this;
        }
    }

    public class SubParameter
    {
        public int Type { get; set; }

        public string Name { get; set; }
    }

    [ActivityResults(typeof(DefaultActivityResult))]
    public class ActivityB : Activity<ParametersB>
    {
        public override ProcessRequirement ProcessRequirement { get; }

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
