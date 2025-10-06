// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.ProcessEngine.Properties;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Production
{
    [Display(Name = nameof(Strings.JobStates_Cleanup), ResourceType = typeof(Strings))]
    internal class CleanUpWaitingState : ProductionJobStateBase
    {
        public override bool CanAbort => true;

        public CleanUpWaitingState(JobDataBase context, StateMap stateMap) 
            : base(context, stateMap, JobClassification.Waiting)
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

        public override void Start()
        {
            NextState(StateCleaningUp);
            Context.CleanupProcesses();
        }

        public override void Complete()
        {
            // Nothing to do here
        }

        public override void Abort()
        {
            // Since we are already cleaning-up, we just want to get rid of this job now...
            // The user may have to clean up by using WTReset afterwards...
            NextState(StateDiscarding);
            Context.InterruptProcesses();
        }

        public override void Interrupt()
        {
            NextState(StateCleanUp);
        }

        public override void ProcessChanged(ProcessData processData, ProcessState trigger)
        {
            // Ignore events here
        }
    }
}
