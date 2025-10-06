// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.ProcessEngine.Properties;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Production
{
    [Display(Name = nameof(Strings.JobStates_Running), ResourceType = typeof(Strings))]
    internal sealed class RunningState : ProductionJobStateBase
    {
        public override bool CanComplete => true;

        public override bool CanAbort => true;

        public RunningState(JobDataBase context, StateMap stateMap)
            : base(context, stateMap, JobClassification.Running)
        {
        }

        public override void Load()
        {
            PerformCleanup();
        }

        public override void Stop()
        {
            NextState(StateSuspended);
            Context.DiscardCachedProcess();
        }

        public override void Complete()
        {
            NextState(StateCompleting);
            Context.DiscardCachedProcess();
        }

        public override void Abort()
        {
            NextState(StateAborting);
            Context.AbortProcesses();
        }

        public override void Interrupt()
        {
            NextState(StateInterrupting);
            Context.InterruptProcesses();
        }

        public override void ProcessChanged(ProcessData processData, ProcessState trigger)
        {
            // If process success or failed complete it (increase counts, remove from job)
            if (trigger == ProcessState.Success || trigger == ProcessState.Failure)
            {
                Context.ProcessCompleted(processData);
            }
            // If process is switched to running, the job amount is not reached, and it is the lastest process
            // dispatch a new process
            else if (trigger == ProcessState.Running && !Context.AmountReached && Context.IsLatestProcess(processData))
            {
                Context.StartProcess();
                Context.ProcessRunning(processData);
            }
            // If process is switched to running, and the amount is reached - switch to completing state
            else if (trigger == ProcessState.Running && Context.AmountReached)
            {
                Context.ProcessRunning(processData);
                NextState(StateCompleting);
            }
        }
    }
}
