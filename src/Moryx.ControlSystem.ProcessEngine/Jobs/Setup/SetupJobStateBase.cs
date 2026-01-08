// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.Recipes;
using Moryx.StateMachines;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Setup;

internal abstract class SetupJobStateBase : JobStateBase
{
    protected new SetupJobData Context => (SetupJobData)base.Context;

    protected SetupJobStateBase(JobDataBase context, StateMap stateMap, JobClassification classification)
        : base(context, stateMap, classification)
    {
    }

    public virtual bool RecipeRequired => false;

    public virtual void UpdateSetup(SetupRecipe newRecipe)
    {
        InvalidJobState();
    }

    public virtual void UnBlockRetrySetup()
    {
        InvalidJobState();
    }

    [StateDefinition(typeof(InitialState), IsInitial = true)]
    protected const int StateInitial = InitialKey;

    [StateDefinition(typeof(WaitingState))]
    protected const int StateWaiting = 10;

    [StateDefinition(typeof(RunningState))]
    protected const int StateRunning = 20;

    [StateDefinition(typeof(InterruptingState))]
    protected const int StateInterrupting = 30;

    [StateDefinition(typeof(RequestRecipeState))]
    protected const int StateRequestRecipe = 40;

    [StateDefinition(typeof(RetrySetupBlockedState))]
    protected const int StateRetrySetupBlocked = 50;

    [StateDefinition(typeof(AbortingSetupState))]
    protected const int StateAborting = 100;

    [StateDefinition(typeof(CompletedState))]
    protected const int StateCompleted = CompletedKey;
}