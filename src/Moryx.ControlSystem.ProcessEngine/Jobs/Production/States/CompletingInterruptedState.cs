// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.ControlSystem.Jobs;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Production
{
    [DisplayName("Interrupted")]
    internal sealed class CompletingInterruptedState : ProductionJobStateBase
    {
        public override bool CanAbort => true;

        public override bool IsStable => true;

        public CompletingInterruptedState(JobDataBase context, StateMap stateMap)
            : base(context, stateMap, JobClassification.Idle)
        {
        }

        public override void Load()
        {
            Context.LoadProcesses();
        }

        public override void Ready()
        {
            NextState(StateCompletingInterruptedWaiting);
        }

        public override void Interrupt()
        {
            // In case the job was not reactivated before shutdown, we ignore the call
        }

        public override void Complete()
        {
            // Nothing to do here we are already completing
        }

        public override void Abort()
        {
            NextState(StateCleanUp);
        }
    }
}
