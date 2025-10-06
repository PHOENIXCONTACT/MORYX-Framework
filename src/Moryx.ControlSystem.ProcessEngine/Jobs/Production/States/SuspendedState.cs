// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Processes;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Production
{
    [DisplayName("Running")]
    internal sealed class SuspendedState : ProductionJobStateBase
    {
        public override bool CanComplete => true;

        public override bool CanAbort => true;

        public override bool IsStable => true;

        public SuspendedState(JobDataBase context, StateMap stateMap)
            : base(context, stateMap, JobClassification.Running)
        {
        }

        public override void Load()
        {
            PerformCleanup();
        }

        public override void Start()
        {
            NextState(StateRunning);
            Context.StartProcess();
        }

        public override void Stop()
        {
            // We are suspending and waiting for the last processes
        }

        public override void Complete()
        {
            NextState(StateCompleting);
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
            // if process was success or failed, complete it
            // if we have no further processes, the job goes back to waiting
            if (trigger >= ProcessState.Discarded)
            {
                Context.ProcessCompleted(processData);

                if (Context.RunningProcesses.Count == 0)
                {
                    NextState(StateWaiting);
                }
            }
            // While suspending, the last process can switch to running 
            // we move to completing to complete the job
            else if (trigger == ProcessState.Running && Context.AmountReached)
            {
                Context.ProcessRunning(processData);
                NextState(StateCompleting);
            }
        }
    }
}

