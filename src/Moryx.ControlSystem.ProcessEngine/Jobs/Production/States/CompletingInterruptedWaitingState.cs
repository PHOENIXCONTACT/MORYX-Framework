// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Properties;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Production
{
    [Display(Name = nameof(Strings.JobStates_Waiting), ResourceType = typeof(Strings))]
    internal sealed class CompletingInterruptedWaitingState : ProductionJobStateBase
    {
        public override bool CanAbort => true;

        public CompletingInterruptedWaitingState(JobDataBase context, StateMap stateMap)
            : base(context, stateMap, JobClassification.Waiting)
        {
        }

        public override void Load()
        {
            // This should not be happen on a clean shutdown but we are gnädig
            NextState(StateCompletingInterrupted);
            Context.LoadProcesses();
        }

        public override void Start()
        {
            NextState(StateCompleting);
            Context.ResumeProcesses();
        }

        public override void Complete()
        {
            // We are already waiting for Completing
        }

        public override void Abort()
        {
            NextState(StateAborting);
            Context.CleanupProcesses();
        }

        public override void Interrupt()
        {
            NextState(StateCompletingInterrupting);
            Context.InterruptProcesses();
        }
    }
}
