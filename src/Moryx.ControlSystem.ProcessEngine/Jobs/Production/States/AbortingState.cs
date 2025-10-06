// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.ProcessEngine.Properties;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Production
{
    [Display(Name = nameof(Strings.JobStates_Aborting), ResourceType = typeof(Strings))]
    internal sealed class AbortingState : ProductionJobStateBase
    {
        public AbortingState(JobDataBase context, StateMap stateMap) 
            : base(context, stateMap, JobClassification.Completing)
        {
        }

        public override void Load()
        {
            PerformCleanup();
        }

        public override void Complete()
        {
            // We are already aborting and this is a kind of completing
        }

        public override void Abort()
        {
            //We are already aborting
        }

        public override void Interrupt()
        {
            NextState(StateCleaningUp);
        }

        public override void ProcessChanged(ProcessData processData, ProcessState trigger)
        {
            // if process was aborted (interrupted), success or failed, complete it
            // if we have no further processes, the job is completed
            if (trigger >= ProcessState.Discarded)
            {
                Context.ProcessCompleted(processData);

                if (Context.RunningProcesses.Count == 0)
                {
                    NextState(StateCompleted);
                }
            }
        }
    }
}
