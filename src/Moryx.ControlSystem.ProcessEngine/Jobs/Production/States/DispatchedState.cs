using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.ProcessEngine.Properties;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Production
{

    [Display(Name = nameof(Strings.JobStates_Dispatched), ResourceType = typeof(Strings))]
    internal sealed class DispatchedState : ProductionJobStateBase
    {
        public override bool CanComplete => true;

        public override bool CanAbort => true;

        public DispatchedState(JobDataBase context, StateMap stateMap)
            : base(context, stateMap, JobClassification.Running)
        {
        }

        public override void Load()
        {
            // Sure, this wasn't a proper shutdown, but a cached process wasn't saved anyway
            // Let's just pretend nothing happened and continue
            NextState(StateInitial);
        }

        public override void Stop()
        {
            // We have dispatched a process in OnEnter but it isn't running
            // Tell the dispatcher to remove the process
            NextState(StateWaiting);
            Context.DiscardCachedProcess();
        }

        public override void Complete()
        {
            // We have dispatched a process but it isn't running
            // Tell the dispatcher to remove the process
            NextState(StateCompleting);
            Context.DiscardCachedProcess();
        }

        public override void Abort()
        {
            // We have dispatched a process but it isn't running
            // Tell the dispatcher to remove the process
            NextState(StateDiscarding);
            Context.InterruptProcesses();
            Context.DiscardCachedProcess();
        }

        public override void Interrupt()
        {
            // We have dispatched a process but it isn't running
            // Tell the dispatcher to remove the process
            NextState(StateInitial);
            Context.DiscardCachedProcess();
        }

        public override void ProcessChanged(ProcessData processData, ProcessState trigger)
        {
            // Wait for running processes
            if (trigger == ProcessState.Running)
            {
                // If amount is reached, job is completing
                if (Context.AmountReached)
                {
                    Context.ProcessRunning(processData);
                    NextState(StateCompleting);
                }
                // The amount is not reached, change to running
                // Dispatch next process if the latest process was switched to running
                else if (Context.IsLatestProcess(processData))
                {
                    NextState(StateRunning);
                    Context.StartProcess();
                    Context.ProcessRunning(processData);
                }
                else
                {
                    NextState(StateRunning);
                }
            }
            else if (trigger == ProcessState.Discarded)
            {
                // Only if job will be stopped and instantly started again
                Context.ProcessCompleted(processData);
            }
            else
            {
                // Throw invalid state exception
                base.ProcessChanged(processData, trigger);
            }
        }
    }
}