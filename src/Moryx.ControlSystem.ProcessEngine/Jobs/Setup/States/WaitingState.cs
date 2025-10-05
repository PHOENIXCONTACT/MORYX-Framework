// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Properties;
using Moryx.ControlSystem.Recipes;
using Moryx.ControlSystem.Setups;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Setup
{

    [Display(Name = nameof(Strings.JobStates_Waiting), ResourceType = typeof(Strings))]
    internal class WaitingState : SetupJobStateBase
    {
        public WaitingState(JobDataBase context, StateMap stateMap)
            : base(context, stateMap, JobClassification.Waiting)
        {
        }

        public override void Load()
        {
            NextState(StateCompleted);
        }

        public override void Start()
        {
            if (Context.Recipe.Execution == SetupExecution.BeforeProduction)
            {
                // PreExeuction can be started directly
                NextState(StateRunning);
                Context.StartProcess();
            }
            else if (Context.Recipe.Execution == SetupExecution.AfterProduction)
            {
                // PostExecution recipe is only a placeholder and created just in time
                NextState(StateRequestRecipe);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public override void UpdateSetup(ISetupRecipe newRecipe)
        {
            if (newRecipe == null)
                // Empty recipe means setup is no longer necessary
                NextState(StateCompleted);
            else
                // Until the process is started, we can still update the recipe
                Context.UpdateRecipe(newRecipe);
        }

        public override void Interrupt()
        {
            NextState(StateCompleted);
        }

        public override void Abort()
        {
            NextState(StateCompleted);
        }
    }
}

