// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Jobs;
using Moryx.StateMachines;
using Moryx.Users;

namespace Moryx.Orders.Management;

internal abstract class OperationDataStateBase : AsyncStateBase<OperationData>, IOperationState
{
    public const int CompletedKey = 80;

    public OperationStateClassification Classification { get; private set; }

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

    protected OperationDataStateBase(OperationData context, StateMap stateMap, OperationStateClassification classification) : base(context, stateMap)
    {
        Classification = classification;
    }

    public virtual Task Assign() => InvalidStateAsync();

    public virtual Task AssignCompleted(bool success) => InvalidStateAsync();

    public virtual Task Resume() => InvalidStateAsync();

    public virtual Task Abort() => InvalidStateAsync();

    public virtual Task IncreaseTargetBy(int amount, User user) => InvalidStateAsync();

    public virtual Task DecreaseTargetBy(int amount, User user) => InvalidStateAsync();

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

    public virtual Task Interrupt(User user) => InvalidStateAsync();

    public virtual Task Report(OperationReport report) => InvalidStateAsync();

    public virtual Task Advice(OperationAdvice advice) => InvalidStateAsync();

    public virtual Task JobsUpdated(JobStateChangedEventArgs args) => InvalidStateAsync();

    public virtual Task ProgressChanged(Job job)
    {
        return Task.CompletedTask;
    }

    public OperationStateClassification GetFullClassification()
    {
        var classification = this.Classification;

        // TODO: This overrides the base classification, rework classification in next major. GitHub issue: #1066.
        if (IsFailed)
            classification = OperationStateClassification.Failed;
        if (IsAssigning)
            classification = OperationStateClassification.Assigning;

        if (CanAssign && Context.AssignState.HasFlag(OperationAssignState.Changed))
            classification |= OperationStateClassification.CanReload;
        if (CanBegin)
            classification |= OperationStateClassification.CanBegin;
        if (CanInterrupt)
            classification |= OperationStateClassification.CanInterrupt;
        if (CanPartialReport || CanFinalReport)
            classification |= OperationStateClassification.CanReport;
        if (CanAdvice)
            classification |= OperationStateClassification.CanAdvice;
        if (IsAmountReached)
            classification |= OperationStateClassification.IsAmountReached;

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
