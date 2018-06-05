using Marvin.Workflows;
using Marvin.Workflows.Transitions;
using Marvin.Workflows.WorkplanSteps;

namespace Marvin.Tests.Workflows
{
    public class InvalidStep : WorkplanStepBase
    {
        /// 
        public override string Name
        {
            get { return "Invalid"; }
        }

        private InvalidStep()
        {
            
        }

        ///
        protected override TransitionBase Instantiate(IWorkplanContext context)
        {
            return new DummyTransition();
        }
    }
}