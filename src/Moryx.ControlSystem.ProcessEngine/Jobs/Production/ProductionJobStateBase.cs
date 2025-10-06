// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ControlSystem.Jobs;
using Moryx.StateMachines;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Production
{
    internal abstract class ProductionJobStateBase : JobStateBase
    {
        protected new ProductionJobData Context => (ProductionJobData)base.Context;

        protected ProductionJobStateBase(JobDataBase context, StateMap stateMap, JobClassification classification)
            : base(context, stateMap, classification)
        {
        }

        protected void PerformCleanup()
        {
            Context.LoadProcesses();
            if (Context.RunningProcesses.Any())
                NextState(StateCleanUp);
            else
                NextState(StateCompleted);
        }

        [StateDefinition(typeof(InitialState), IsInitial = true)]
        protected const int StateInitial = InitialKey;

        [StateDefinition(typeof(WaitingState))]
        protected const int StateWaiting = 5;

        [StateDefinition(typeof(DispatchedState))]
        protected const int StateDispatched = 10;

        [StateDefinition(typeof(SuspendedState))]
        protected const int StateSuspended = 20;

        [StateDefinition(typeof(RunningState))]
        protected const int StateRunning = 50;

        [StateDefinition(typeof(InterruptedWaitingState))]
        protected const int StateInterruptedWaiting = 60;

        [StateDefinition(typeof(InterruptedState))]
        protected const int StateInterrupted = 65;

        [StateDefinition(typeof(InterruptingState))]
        protected const int StateInterrupting = 70;

        [StateDefinition(typeof(CompletingState))]
        protected const int StateCompleting = 90;

        [StateDefinition(typeof(CompletingInterruptedWaitingState))]
        protected const int StateCompletingInterruptedWaiting = 100;

        [StateDefinition(typeof(CompletingInterruptedState))]
        protected const int StateCompletingInterrupted = 105;

        [StateDefinition(typeof(CompletingInterruptingState))]
        protected const int StateCompletingInterrupting = 110;

        [StateDefinition(typeof(AbortingState))]
        protected const int StateAborting = 120;

        [StateDefinition(typeof(CleanUpState))]
        protected const int StateCleanUp = 200;

        [StateDefinition(typeof(CleanUpWaitingState))]
        protected const int StateCleanUpWaiting = 210;

        [StateDefinition(typeof(CleaningUpState))]
        protected const int StateCleaningUp = 230;

        [StateDefinition(typeof(DiscardingState))]
        protected const int StateDiscarding = 240;

        [StateDefinition(typeof(CompletedState))]
        protected const int StateCompleted = CompletedKey;
    }
}
