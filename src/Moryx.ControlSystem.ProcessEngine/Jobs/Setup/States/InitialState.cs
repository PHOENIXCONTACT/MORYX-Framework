using System.ComponentModel.DataAnnotations;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Properties;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Setup
{
    [Display(Name = nameof(Strings.JobStates_Initial), ResourceType = typeof(Strings))]
    internal class InitialState : SetupJobStateBase
    {
        public override bool IsStable => true;

        public InitialState(JobDataBase context, StateMap stateMap)
            : base(context, stateMap, JobClassification.Idle)
        {
        }

        public override void Load()
        {
            NextState(StateCompleted);
        }

        public override void Ready()
        {
            NextState(StateWaiting);
        }

        public override void Interrupt()
        {
            NextState(StateCompleted);
        }

        public override void Abort()
        {
            NextState(StateCompleted);
        }
    }
}