using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Properties;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Production
{
    [Display(Name = nameof(Strings.JobStates_Waiting), ResourceType = typeof(Strings))]
    internal sealed class InterruptedWaitingState : ProductionJobStateBase
    {
        public override bool CanComplete => true;

        public override bool CanAbort => true;

        public InterruptedWaitingState(JobDataBase context, StateMap stateMap)
            : base(context, stateMap, JobClassification.Waiting)
        {
        }

        public override void Load()
        {
            // This should not be happen on a clean shutdown but we are gnädig
            NextState(StateInterrupted);
            Context.LoadProcesses();
        }

        public override void Start()
        {
            NextState(StateRunning);
            Context.StartProcess(); // Start first to append running processes for IsLatestProcess
            Context.ResumeProcesses();
        }

        public override void Complete()
        {
            // If this job was never started, switch to Completed
            if (Context.RunningProcesses.Count == 0)
            {
                NextState(StateCompleted);
            }
            else
            {
                NextState(StateCompletingInterruptedWaiting);
            }
        }

        public override void Abort()
        {
            if (Context.RunningProcesses.Count == 0)
            {
                NextState(StateCompleted);
            }
            else
            {
                NextState(StateAborting);
                Context.CleanupProcesses();
            }
        }

        public override void Interrupt()
        {
            if (Context.RunningProcesses.Count == 0)
            {
                NextState(StateInterrupted);
            }
            else
            {
                NextState(StateInterrupting);
                Context.InterruptProcesses();
            }
        }
    }
}