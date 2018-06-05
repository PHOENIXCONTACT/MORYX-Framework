using Marvin.Workflows;
using Marvin.Workflows.Transitions;
using Marvin.Workflows.WorkplanSteps;

namespace Marvin.Tests.Workflows
{
    public class EmptyParameters
    {
        public int Hidden { get; set; } 
    }

    public class EmptyParametersStep : WorkplanStepBase
    {
        /// 
        public override string Name
        {
            get { return "EmptyParameters"; }
        }

        [Initializer]
        public EmptyParameters Parameters { get; set; }

        ///
        protected override TransitionBase Instantiate(IWorkplanContext context)
        {
            return null;
        }
    }
}