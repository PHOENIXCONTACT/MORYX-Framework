// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.ProcessEngine.Properties;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Setup
{

    [Display(Name = nameof(Strings.JobStates_Interrupting), ResourceType = typeof(Strings))]
    internal class InterruptingState : SetupJobStateBase
    {
        public InterruptingState(JobDataBase context, StateMap stateMap)
            : base(context, stateMap, JobClassification.Completing)
        {
        }

        public override void Load()
        {
            NextState(StateCompleted);
        }

        public override void Start()
        {

        }

        public override void Stop()
        {

        }

        public override void ProcessChanged(ProcessData processData, ProcessState trigger)
        {
            if (trigger >= ProcessState.Interrupted)
                NextState(StateCompleted);
        }
    }
}
