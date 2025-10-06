// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.ProcessEngine.Properties;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Production
{
    [Display(Name = nameof(Strings.JobStates_Waiting), ResourceType = typeof(Strings))]
    internal sealed class WaitingState : ProductionJobStateBase
    {
        public override bool CanComplete => true;

        public override bool CanAbort => true;

        public WaitingState(JobDataBase context, StateMap stateMap)
            : base(context, stateMap, JobClassification.Waiting)
        {

        }

        public override void Load()
        {
            NextState(StateInitial);
        }

        public override void Start()
        {
            NextState(StateDispatched);
            Context.StartProcess();
        }

        public override void Complete()
        {
            NextState(StateCompleted);
        }

        public override void Abort()
        {
            NextState(StateCompleted);
        }

        public override void Interrupt()
        {
            NextState(StateInitial);
        }

        public override void ProcessChanged(ProcessData processData, ProcessState trigger)
        {
            if (trigger == ProcessState.Discarded)
            {
                // Discarded processes can occur when a Dispatched job was suspended again
                Context.ProcessCompleted(processData);
            }
            else
            {
                // No other state changes may occur
                base.ProcessChanged(processData, trigger);
            }
        }
    }
}
