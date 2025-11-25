// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Jobs;
using Moryx.StateMachines;
using Moryx.Users;

namespace Moryx.Orders.Management
{
    internal abstract class OperationDataStateBase : StateBase<OperationData>, IOperationState
    {
        public const int CompletedKey = 80;

        public OperationClassification Classification { get; private set; }

        int IOperationState.Key => Key;

        public virtual bool CanAssign => false;

        public virtual bool CanBegin => false;

        public virtual bool CanReduceAmount => false;

        public virtual bool CanInterrupt => false;

        public virtual bool CanPartialReport => false;

        public virtual bool CanFinalReport => false;

        public virtual bool CanAdvice => false;

        public virtual bool IsFailed => false;

        public virtual bool IsCreated => true;

        public virtual bool IsAssigning => false;

        public virtual bool IsAmountReached => false;

        protected OperationDataStateBase(OperationData context, StateMap stateMap, OperationClassification classification) : base(context, stateMap)
        {
            Classification = classification;
        }

        public virtual void Assign()
        {
            InvalidState();
        }

        public virtual void AssignCompleted(bool success) => InvalidState();

        public virtual void Resume() => InvalidState();

        public virtual void Abort() => InvalidState();

        public virtual void IncreaseTargetBy(int amount, User user) => InvalidState();

        public virtual void DecreaseTargetBy(int amount, User user) => InvalidState();

        public virtual void Dispatched() => InvalidState();

        public virtual ReportContext GetReportContext()
        {
            InvalidState();
            return null;
        }

        public virtual AdviceContext GetAdviceContext()
        {
            InvalidState();
            return null;
        }

        public virtual void Interrupt(OperationReport report) => InvalidState();

        public virtual void Report(OperationReport report) => InvalidState();

        public virtual void Advice(OperationAdvice advice) => InvalidState();

        public virtual void JobsUpdated(JobStateChangedEventArgs args) => InvalidState();

        public virtual void UpdateRecipe(IRecipe recipe) => InvalidState();

        public virtual void UpdateRecipes(IReadOnlyList<IProductRecipe> recipes) => InvalidState();

        public virtual void ProgressChanged(Job job)
        {
        }

        public OperationClassification GetFullClassification()
        {
            var classification = this.Classification;
            if (IsFailed)
                classification = OperationClassification.Failed;
            if (IsAssigning)
                classification = OperationClassification.Assigning;

            if (CanAssign && Context.AssignState.HasFlag(OperationAssignState.Changed))
                classification |= OperationClassification.CanReload;
            if (CanBegin)
                classification |= OperationClassification.CanBegin;
            if (CanInterrupt)
                classification |= OperationClassification.CanInterrupt;
            if (CanPartialReport || CanFinalReport)
                classification |= OperationClassification.CanReport;
            if (CanAdvice)
                classification |= OperationClassification.CanAdvice;
            if (IsAmountReached)
                classification |= OperationClassification.IsAmountReached;

            return classification;
        }

        [StateDefinition(typeof(InitialState), IsInitial = true)]
        protected const int StateInitial = 0;

        [StateDefinition(typeof(InitialAssignState))]
        protected const int StateInitialAssign = 10;

        [StateDefinition(typeof(InitialAssignFailedState))]
        protected const int StateInitialAssignFailed = 20;

        [StateDefinition(typeof(ReadyState))]
        protected const int StateReady = 30;

        [StateDefinition(typeof(ReadyAssignState))]
        protected const int StateReadyAssign = 32;

        [StateDefinition(typeof(ReadyAssignFailedState))]
        protected const int StateReadyAssignFailed = 34;

        [StateDefinition(typeof(InterruptingState))]
        protected const int StateInterrupting = 40;

        [StateDefinition(typeof(InterruptedState))]
        protected const int StateInterrupted = 50;

        [StateDefinition(typeof(InterruptedAssignState))]
        protected const int StateInterruptedAssign = 52;

        [StateDefinition(typeof(InterruptedAssignFailedState))]
        protected const int StateInterruptedAssignFailed = 54;

        [StateDefinition(typeof(RunningState))]
        protected const int StateRunning = 60;

        [StateDefinition(typeof(AmountReachedState))]
        protected const int StateAmountReached = 70;

        [StateDefinition(typeof(AmountReachedAssignState))]
        protected const int StateAmountReachedAssign = 72;

        [StateDefinition(typeof(AmountReachedAssignFailedState))]
        protected const int StateAmountReachedAssignFailed = 74;

        [StateDefinition(typeof(CompletedState))]
        protected const int StateCompleted = CompletedKey;

        [StateDefinition(typeof(AbortedState))]
        protected const int StateAborted = CompletedKey + 1;

    }
}

