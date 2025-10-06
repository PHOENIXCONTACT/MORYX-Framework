// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.ProcessEngine.Properties;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Production
{

    [Display(Name = nameof(Strings.JobStates_Cleanup), ResourceType = typeof(Strings))]
    internal sealed class CleaningUpState : ProductionJobStateBase
    {
        public override bool CanAbort => true;

        public CleaningUpState(JobDataBase context, StateMap stateMap)
            : base(context, stateMap, JobClassification.Completing)
        {
        }

        public override void Load()
        {
            try
            {
                PerformCleanup();
            }
            catch (Exception)
            {
                // If this crashes all we can do is remove the job
                NextState(StateCompleted);
            }
        }

        public override void Complete()
        {
            // Nothing to do here. Cleaning up already started.
        }

        public override void Abort()
        {
            // Since we are already cleaning up, we just want to get rid of this job now...
            // The user may have to clean up by using WTReset afterwards...
            NextState(StateDiscarding);
            Context.InterruptProcesses();
        }

        public override void Interrupt()
        {
            // Interrupt processes and resume cleanup on next boot to shutdown properly
            Context.InterruptProcesses();
        }

        public override void ProcessChanged(ProcessData processData, ProcessState trigger)
        {
            // Complete failed processes
            if ( trigger >= ProcessState.Discarded)
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

