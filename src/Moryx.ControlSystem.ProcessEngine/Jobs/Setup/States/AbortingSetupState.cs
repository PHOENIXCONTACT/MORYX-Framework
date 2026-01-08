// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.ProcessEngine.Properties;
using System.ComponentModel.DataAnnotations;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Setup
{

    [Display(Name = nameof(Strings.JobStates_Aborting), ResourceType = typeof(Strings))]
    internal class AbortingSetupState : SetupJobStateBase
    {
        public AbortingSetupState(JobDataBase context, StateMap stateMap) : base(context, stateMap, JobClassification.Completing)
        {
        }

        public override void Load()
        {
            NextState(StateCompleted);
        }

        public override void Interrupt()
        {
            // We are already aborting
        }

        public override void ProcessChanged(ProcessData processData, ProcessState trigger)
        {
            if (trigger < ProcessState.Interrupted)
                return;

            Context.ProcessCompleted(processData);
            NextState(StateCompleted);
        }
    }
}
