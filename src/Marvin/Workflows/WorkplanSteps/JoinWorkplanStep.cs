using System.Runtime.Serialization;
using Marvin.Workflows.Transitions;

namespace Marvin.Workflows.WorkplanSteps
{
    /// <summary>
    /// Workplan step to join multiple inputs
    /// </summary>
    [DataContract]
    public class JoinWorkplanStep : WorkplanStepBase
    {
        private JoinWorkplanStep()
        {
        }

        /// 
        public override string Name => "Join";

        /// <summary>
        /// Create new join step for certain number of inputs
        /// </summary>
        /// <param name="inputs">Number of inputs</param>
        public JoinWorkplanStep(ushort inputs = 2)
        {
            Inputs = new IConnector[inputs];
        }

        /// 
        protected override TransitionBase Instantiate(IWorkplanContext context)
        {
            return new JoinTransition();
        }
    }
}