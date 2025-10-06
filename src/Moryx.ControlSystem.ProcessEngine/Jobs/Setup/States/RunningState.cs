// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.ProcessEngine.Properties;
using Moryx.ControlSystem.Recipes;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Setup
{
    [Display(Name = nameof(Strings.JobStates_Running), ResourceType = typeof(Strings))]
    internal class RunningState : SetupJobStateBase
    {
        public RunningState(JobDataBase context, StateMap stateMap)
            : base(context, stateMap, JobClassification.Running)
        {
        }

        public override void Load()
        {
            NextState(StateCompleted);
        }

        public override void Start()
        {

        }

        public override void Stop()
        {

        }

        public override void UpdateSetup(ISetupRecipe newRecipe)
        {
            // Ignore: We get a new setup upon retry
        }

        public override void Interrupt()
        {
            NextState(StateInterrupting);
            Context.InterruptProcesses();
        }

        public override void Abort()
        {
            NextState(StateAborting);
            Context.AbortProcesses();
        }

        public override void ProcessChanged(ProcessData processData, ProcessState trigger)
        {
            if (trigger != ProcessState.Success && trigger != ProcessState.Failure)
                return;

            Context.ProcessCompleted(processData);
            if (trigger == ProcessState.Success)
            {
                NextState(StateCompleted);
            }
            else if (trigger == ProcessState.Failure)
            {
                Context.IncrementRetry();
                // Check if we reached the limit
                if (Context.IsRetryLimitReached())
                    NextState(StateRetrySetupBlocked); // Enter blocked state and await user confirmation
                else
                    NextState(StateRequestRecipe); // Request an updated setup from the setup manager
            }
        }
    }
}
