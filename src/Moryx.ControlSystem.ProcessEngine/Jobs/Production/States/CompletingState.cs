using System.ComponentModel;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Processes;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Production
{
    [DisplayName("Completing")]
    internal sealed class CompletingState : ProductionJobStateBase
    {
        public override bool CanAbort => true;

        public CompletingState(JobDataBase context, StateMap stateMap) 
            : base(context, stateMap, JobClassification.Completing)
        {
        }

        public override void Load()
        {
            PerformCleanup();
        }

        public override void Complete()
        {
            // Already Completing
        }

        public override void Abort()
        {
            NextState(StateAborting);
            Context.AbortProcesses();
        }

        public override void Interrupt()
        {
            NextState(StateCompletingInterrupting);
            Context.InterruptProcesses();
        }

        public override void ProcessChanged(ProcessData processData, ProcessState trigger)
        {
            // Complete process if success or failure and switch state if necessary
            if (trigger >= ProcessState.Discarded)
            {
                Context.ProcessCompleted(processData);

                // If all running processes of a job are finished, switch to Completed
                if (Context.RunningProcesses.Count == 0)
                {
                    NextState(StateCompleted);
                }
            }
        }
    }
}