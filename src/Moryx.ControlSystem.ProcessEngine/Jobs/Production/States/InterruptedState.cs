// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Properties;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Production
{
    [Display(Name = nameof(Strings.JobStates_Interrupted), ResourceType = typeof(Strings))]
    internal sealed class InterruptedState : ProductionJobStateBase
    {
        public override bool CanComplete => true;

        public override bool IsStable => true;

        public override bool CanAbort => true;

        public InterruptedState(JobDataBase context, StateMap stateMap) 
            : base(context, stateMap, JobClassification.Idle)
        {
        }

        public override void Load()
        {
            Context.LoadProcesses();
        }

        public override void Ready()
        {
            NextState(StateInterruptedWaiting);
        }

        public override void Interrupt()
        {
            // In case the job was not reactiveted before shutdown, we ignore the call
        }

        public override void Complete()
        {
            NextState(StateCompletingInterrupted);
        }

        public override void Abort()
        {
            NextState(StateCleanUp);
        }
    }
}
