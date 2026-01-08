// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.ProcessEngine.Properties;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Production;

[Display(Name = nameof(Strings.JobStates_Initial), ResourceType = typeof(Strings))]
internal sealed class InitialState : ProductionJobStateBase
{
    public override bool CanComplete => true;

    public override bool CanAbort => true;

    public override bool IsStable => true;

    public InitialState(JobDataBase context, StateMap stateMap)
        : base(context, stateMap, JobClassification.Idle)
    {
    }

    public override void Load()
    {
        // Nothing to load
    }

    public override void Ready()
    {
        NextState(StateWaiting);
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
        // Nothing to interrupt
        if (Context.Dispatcher.RebootCompleteInitial)
            NextState(StateCompleted);
    }

    public override void ProcessChanged(ProcessData processData, ProcessState trigger)
    {
        if (trigger == ProcessState.Discarded)
        {
            // Discarded processes can occur when a Dispatched job was interrupted again
            Context.ProcessCompleted(processData);
        }
        else
        {
            // No other state changes may occur
            base.ProcessChanged(processData, trigger);
        }
    }
}