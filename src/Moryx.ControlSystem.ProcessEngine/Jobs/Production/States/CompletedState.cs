using System.ComponentModel;
using Moryx.ControlSystem.Jobs;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Production
{
    [DisplayName("Completed")]
    internal sealed class CompletedState : ProductionJobStateBase
    {
        public override bool IsStable => true;

        public CompletedState(JobDataBase context, StateMap stateMap) 
            : base(context, stateMap, JobClassification.Completed)
        {
        }

        public override void Abort()
        {
            // Already completed, nothing to abort
        }

        public override void Complete()
        {
            // Already completed, nothing to do
        }
    }
}