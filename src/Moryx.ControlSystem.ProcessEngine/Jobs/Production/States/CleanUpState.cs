// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.ProcessEngine.Properties;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Production;

[Display(Name = nameof(Strings.JobStates_Cleanup), ResourceType = typeof(Strings))]
internal class CleanUpState : ProductionJobStateBase
{
    public override bool CanAbort => true;

    public CleanUpState(JobDataBase context, StateMap stateMap)
        : base(context, stateMap, JobClassification.Idle)
    {
    }

    public override void Load()
    {
        try
        {
            Context.LoadProcesses();

            if (Context.RunningProcesses.Count == 0)
                NextState(StateCompleted);
        }
        catch (Exception)
        {
            // If this crashes all we can do is remove the job
            NextState(StateCompleted);
        }
    }

    public override void Ready()
    {
        NextState(StateCleanUpWaiting);
    }

    public override void Interrupt()
    {
        // In case the job was not reactivated before shutdown, we ignore the call
    }

    public override void Complete()
    {
        // Simply do nothing
    }

    public override void Abort()
    {
        NextState(StateDiscarding);
        Context.InterruptProcesses();
    }

    public override void ProcessChanged(ProcessData processData, ProcessState trigger)
    {
        // We might receive interruption events here if we came from the waiting state
        // but they are irrelevant
    }
}