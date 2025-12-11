// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.Recipes;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Setup
{
    /// <summary>
    /// In this state a setup job has performed the setup workplan, but something failed so he waits for a new recipe
    /// from the setup manager.
    /// </summary>
    [DisplayName("RetrySetupBlocked")]
    internal class RetrySetupBlockedState : SetupJobStateBase
    {
        public override bool RecipeRequired => true;

        public RetrySetupBlockedState(JobDataBase context, StateMap stateMap) : base(context, stateMap, JobClassification.Running)
        {
        }

        public override void OnEnter()
        {
            Context.NotifyAboutBlockedRetry();
        }

        public override void OnExit()
        {
            Context.ClearNotification();
        }

        public override void Load()
        {
            NextState(StateCompleted);
        }

        public override void Interrupt()
        {
            NextState(StateCompleted);
        }

        public override void UpdateSetup(SetupRecipe newRecipe)
        {
            // We will no longer retry setups
        }

        public override void UnBlockRetrySetup()
        {
            NextState(StateRequestRecipe);
        }

        public override void Abort()
        {
            NextState(StateCompleted);
        }
    }
}
