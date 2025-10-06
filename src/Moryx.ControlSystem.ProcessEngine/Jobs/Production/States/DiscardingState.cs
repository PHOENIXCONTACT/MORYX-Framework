// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Processes;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Production
{
    /// <summary>
    /// Instead of finishing a cleanup/abort we discard processes and the job
    /// </summary>
    internal class DiscardingState : ProductionJobStateBase
    {
        public DiscardingState(JobDataBase context, StateMap stateMap) : base(context, stateMap, JobClassification.Completing)
        {
        }

        public override void Load()
        {
            // After we restart we can simply discard everything
            NextState(StateCompleted);
        }

        public override void Complete()
        {
            // Nothing to do
        }

        public override void Abort()
        {
            // We leave this here till the next restart
        }

        public override void Interrupt()
        {
            // Nothing to do
        }

        public override void ProcessChanged(ProcessData processData, ProcessState trigger)
        {
            // Complete failed processes
            if (trigger >= ProcessState.Interrupted)
            {
                Context.ProcessCompleted(processData);

                // If all running processes of a job are finished, switch to Completed
                if (Context.RunningProcesses.Count == 0)
                {
                    NextState(StateCompleted);
                }
            }
        }
    }
}
