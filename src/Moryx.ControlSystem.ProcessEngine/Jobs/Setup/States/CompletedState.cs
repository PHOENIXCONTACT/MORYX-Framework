using System.ComponentModel;
using Moryx.ControlSystem.Jobs;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Setup
{
    [DisplayName("Completed")]
    internal class CompletedState : SetupJobStateBase
    {
        public override bool IsStable => true;

        public CompletedState(JobDataBase context, StateMap stateMap)
            : base(context, stateMap, JobClassification.Completed)
        {
        }
    }
}