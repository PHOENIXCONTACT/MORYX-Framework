// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.ProcessEngine.Properties;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Production
{
    [Display(Name = nameof(Strings.JobStates_Interrupting), ResourceType = typeof(Strings))]
    internal sealed class CompletingInterruptingState : ProductionJobStateBase
    {
        public CompletingInterruptingState(JobDataBase context, StateMap stateMap)
            : base(context, stateMap, JobClassification.Completing)
        {
        }

        public override void Load()
        {
            PerformCleanup();
        }

        public override void Complete()
        {
            // Already shutting down
        }

        public override void ProcessChanged(ProcessData processData, ProcessState trigger)
        {
            // Only remove completed processes
            if (trigger >= ProcessState.Interrupted)
                Context.ProcessCompleted(processData);

            // Wait till all running processes have reported a stable state
            if (Context.RunningProcesses.Count > 0)
                return;

            // Iterate all processes from the back, looking for any non-completed process
            // If the job actually completed, this WILL iterate the entire list
            var anyInterrupted = false;
            for (var i = Context.AllProcesses.Count - 1; i >= 0; i--)
            {
                if (Context.AllProcesses[i].State > ProcessState.Interrupted)
                    continue;

                anyInterrupted = true;
                break;
            }

            // If we found any interrupted job, we are interrupted, otherwise completed
            if (anyInterrupted)
                NextState(StateCompletingInterrupted);
            else
                NextState(StateCompleted);
        }
    }
}
