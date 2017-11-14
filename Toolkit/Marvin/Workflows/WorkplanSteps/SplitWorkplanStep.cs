using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.Workflows.Transitions;

namespace Marvin.Workflows.WorkplanSteps
{
    /// <summary>
    /// Workplanstep to split execution
    /// </summary>
    [DataContract]
    [Description("Splits the current execution path into 'n' parallel paths.")]
    public class SplitWorkplanStep : WorkplanStepBase
    {
        private SplitWorkplanStep()
        {
        }

        /// <summary>
        /// Create new split instance
        /// </summary>
        /// <param name="outputs"></param>
        public SplitWorkplanStep([Initializer("Outputs", Description = "Number of parallel paths")]int outputs = 2)
        {
            if (outputs <= 1)
                throw new ArgumentException("Split must have at least two outputs!");

            Outputs = new IConnector[outputs];
            OutputDescriptions = new OutputDescription[outputs];
            for (int i = 0; i < outputs; i++)
            {
                OutputDescriptions[i] = new OutputDescription{Success = true};
            }
        }

        /// <summary>
        /// Transition name
        /// </summary>
        public override string Name
        {
            get { return "Split"; }
        }

        /// <summary>
        /// Create transistion instance
        /// </summary>
        protected override TransitionBase Instantiate(IWorkplanContext context)
        {
            return new SplitTransition();
        }
    }
}