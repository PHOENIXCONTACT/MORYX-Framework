// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.ProcessEngine.Properties;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Production
{
    [Display(Name = nameof(Strings.JobStates_Interrupting), ResourceType = typeof(Strings))]
    internal sealed class InterruptingState : ProductionJobStateBase
    {
        public InterruptingState(JobDataBase context, StateMap stateMap) 
            : base(context, stateMap, JobClassification.Running)
        {
        }

        public override void Load()
        {
            PerformCleanup();
        }

        public override void Stop()
        {
            // We are interrupting
        }

        public override void ProcessChanged(ProcessData processData, ProcessState trigger)
        {
            // if process was aborted (interrupted), success or failed, complete it
            // if we have no further processes, the job is interrupted
            if (trigger >= ProcessState.Interrupted)
            {
                Context.ProcessCompleted(processData);

                if (Context.RunningProcesses.Count == 0)
                {
                    NextState(StateInterrupted);
                }
            }
        }
    }
}
