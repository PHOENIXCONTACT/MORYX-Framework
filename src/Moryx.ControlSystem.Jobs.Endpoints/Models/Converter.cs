// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Recipes;
using Moryx.AbstractionLayer.Workplans;
using Moryx.ControlSystem.Recipes;
using Moryx.ControlSystem.Setups;

namespace Moryx.ControlSystem.Jobs.Endpoints;

internal static class Converter
{
    internal static StateChangeModel ToModel(JobStateChangedEventArgs eventArgs)
    {
        return new StateChangeModel()
        {
            JobModel = Converter.ToModel(eventArgs.Job),
            CurrentState = eventArgs.CurrentState,
            PreviousState = eventArgs.PreviousState
        };
    }

    internal static JobModel ToModel(Job job)
    {
        var model = new JobModel
        {
            Id = job.Id,
            RecipeId = job.Recipe.Id,
            CanAbort = job.Classification < JobClassification.Completed,
            IsRunning = job.Classification == JobClassification.Running |
                        job.Classification == JobClassification.Completing,
            IsWaiting = job.Classification == JobClassification.Waiting,
            Workplan = (job.Recipe as IWorkplanRecipe)?.Workplan.Name,
            State = job.Classification.ToString(),
            DisplayState = job.StateDisplayName
        };

        if (job.Recipe is ProductionRecipe)
        {
            model.ProductionJob = new ProductionJobModel
            {
                ProductIdentity = (job.Recipe as ProductionRecipe)?.Target.Identity.ToString(),
                Amount = job.Amount,
                SuccessCount = job.SuccessCount,
                FailureCount = job.FailureCount,
                RunningCount = job.RunningProcesses.Count,
                ReworkedCount = job.ReworkedCount,
            };

            model.CanComplete = job.Classification < JobClassification.Completing;
        }
        else if (job.Recipe is SetupRecipe setupRecipe)
        {
            var process = job.RunningProcesses.FirstOrDefault();
            model.SetupJob = new SetupJobModel
            {
                Classification = setupRecipe.Execution == SetupExecution.BeforeProduction
                    ? SetupJobClassification.Prepare
                    : SetupJobClassification.Cleanup,
                TargetRecipeId = setupRecipe.TargetRecipe.Id,
                Steps = setupRecipe.Workplan.Steps.OfType<ITaskStep<IParameters>>().Select(step => new SetupStepModel
                {
                    Name = step.Name,
                    State = ConvertActivityState(job, step.Id)
                }).ToArray()
            };
            model.CanComplete = false;
        }
        else
        {
            throw new ArgumentException("Unsupported job type", nameof(job));
        }

        return model;
    }

    private static StepState ConvertActivityState(Job job, long stepId)
    {
        var activities = job.RunningProcesses.SelectMany(p => p.GetActivities().OfType<Activity>()).ToList();
        var activity = activities.FirstOrDefault(a => a.StepId == stepId);
        return activity != null ? GetStepState(activity.Tracing as Tracing) : StepState.Initial;
    }

    private static StepState GetStepState(Tracing tracing)
    {
        if (tracing.Completed is not null)
            return StepState.Completed;

        if (tracing.Started is not null)
            return StepState.Running;

        return StepState.Initial;
    }
}