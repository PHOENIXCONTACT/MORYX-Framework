
// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.ControlSystem.Recipes;
using Moryx.ControlSystem.Setups;
using Moryx.Logging;
using Moryx.Workplans;
using Moryx.Workplans.WorkplanSteps;

namespace Moryx.ControlSystem.SetupProvider;

[Component(LifeCycle.Singleton, typeof(ISetupManager))]
internal class SetupManager : ISetupManager, ILoggingComponent
{
    private readonly ICollection<ISetupTrigger> _triggers = new List<ISetupTrigger>();

    #region Dependencies

    public IModuleLogger Logger { get; set; }

    /// <summary>
    /// Castle factory to create <see cref="ISetupTrigger"/> instances from
    /// their <see cref="SetupTriggerConfig"/>
    /// </summary>
    public ISetupTriggerFactory TriggerFactory { get; set; }

    /// <summary>
    /// Module config with the triggers
    /// </summary>
    public ModuleConfig Config { get; set; }

    #endregion

    /// <inheritdoc />
    public void Start()
    {
        foreach (var triggerConfig in Config.SetupTriggers)
        {
            var trigger = TriggerFactory.Create(triggerConfig);
            _triggers.Add(trigger);
        }
    }

    /// <inheritdoc />
    public void Stop()
    {
    }

    public SetupRecipe RequiredSetup(SetupExecution execution, ProductionRecipe recipe, ISetupTarget targetSystem)
    {
        Logger.LogTrace(
            "Creating required {execution} {setupType} for running production recipe '{productionRecipeName}' (Id={productionRecipeId})",
            execution,
            nameof(SetupRecipe),
            recipe?.Name,
            recipe?.Id
        );

        // Determine the triggers
        var triggers = new List<ISetupTrigger>();

        // TODO: This loop restores old behavior with the new API, but it has much more potential
        foreach (var trigger in _triggers.Where(t => t.Execution == execution))
        {
            using var triggerScope = Logger.BeginScope(new Dictionary<string, object>
            {
                ["trigger"] = trigger?.ToString(),
                ["triggerType"] = trigger?.GetType().Name
            });

            var evaluation = TryEvaluate(trigger, recipe);

            if (!evaluation.Required)
            {
                Logger.LogTrace("Evaluation not required for trigger {trigger}", trigger);
                continue;
            }

            if (evaluation is SetupEvaluation.Change change)
            {
                if (execution == SetupExecution.BeforeProduction)
                {
                    var targetCells = targetSystem.Cells(change.TargetCapabilities);
                    var hasTargetCaps = targetCells.Any();

                    if (hasTargetCaps)
                    {
                        if (Logger.IsEnabled(LogLevel.Trace))
                        {
                            Logger.LogTrace(
                                "Target already provides required capabilities {capabilities} (cells=[{cells}])",
                                change.TargetCapabilities,
                                string.Join(", ", targetCells));
                        }
                        else
                        {
                            Logger.LogTrace(
                                "Target already provides required capabilities {capabilities}",
                                change.TargetCapabilities);
                        }
                        continue;
                    }
                }
                else if (execution == SetupExecution.AfterProduction)
                {
                    var currentCells = targetSystem.Cells(change.CurrentCapabilities);
                    var hasCurrentCaps = currentCells.Any();

                    if (!hasCurrentCaps)
                    {
                        if (Logger.IsEnabled(LogLevel.Trace))
                        {
                            Logger.LogTrace(
                                "Target does not provide current capabilities {capabilities} (cells=[{cells}])",
                                change.CurrentCapabilities,
                                string.Join(", ", currentCells));
                        }
                        else
                        {
                            Logger.LogTrace(
                                "Target does not provide current capabilities {capabilities}",
                                change.CurrentCapabilities);
                        }
                        continue;
                    }
                }
            }
            triggers.Add(trigger);
            Logger.LogTrace(
                "Accepted trigger {trigger} (evaluationType={evaluationType}, execution={execution})",
                trigger, evaluation.GetType().Name, execution);
        }

        Logger.LogDebug("Evaluation summary: {evaluated} triggers checked for execution '{execution}', {selected} selected for step creation",
            _triggers.Count(t => t.Execution == execution), execution, triggers.Count);

        // Create all necessary setup steps
        var stepGroups = new Dictionary<int, List<IWorkplanStep>>();
        foreach (var trigger in triggers.OrderBy(t => t.SortOrder))
        {
            var index = trigger.SortOrder;
            // Create step collection for first entry of a sort order
            if (!stepGroups.TryGetValue(index, out var value))
            {
                value = new List<IWorkplanStep>();
                stepGroups[index] = value;
            }

            value.AddRange(TryCreateSteps(trigger, recipe));

            if (value.Count == 0)
            {
                Logger.LogWarning("{Trigger} with sort index {sortOrder} found the system to require a setup {executionType}, but did not create workplan steps.",
                    nameof(trigger), index, trigger.Execution);
                stepGroups.Remove(index);
            }
        }
        if (stepGroups.Count == 0) // No setup necessary for this recipe
        {
            return null;
        }

        Logger.LogDebug("Created {totalSteps} workplan steps in {totalGroups}. Wiring workplan...",
            stepGroups.Values.Sum(g => g.Count), stepGroups.Count);

        // Add steps to the workplan
        var workplan = new Workplan
        {
            Name = $"{(execution == SetupExecution.BeforeProduction ? "Before" : "After")} {recipe.Name}",
            Version = 1,
            State = WorkplanState.Released
        };
        workplan.Add(stepGroups.SelectMany(sg => sg.Value).ToArray());

        // Wire steps within the workplan
        WireWorkplan(workplan, stepGroups);

        // Create a setup recipe
        var setupRecipe = new SetupRecipe
        {
            Execution = execution,
            SetupClassification = stepGroups.SelectMany(sg => sg.Value)
                .Aggregate(SetupClassification.Unspecified, (current, step) => current | (step as ISetupStep)?.Classification ?? SetupClassification.Unspecified),
            Workplan = workplan,
            Name = workplan.Name,
            TargetRecipe = recipe,
            Classification = RecipeClassification.Default
        };
        return setupRecipe;
    }


    private SetupEvaluation TryEvaluate(ISetupTrigger trigger, ProductionRecipe recipe)
    {
        Logger.LogTrace("Entering TryEvaluate (trigger={trigger}, recipeId={recipeId}, recipeName={recipeName})",
            trigger, recipe?.Id, recipe?.Name);

        try
        {
            var result = trigger.Evaluate(recipe);

            Logger.LogTrace("Evaluation result (trigger={trigger}, required={required}, evaluationType={evaluationType})",
                trigger, result.Required, result.GetType().Name);

            return result;
        }
        catch (Exception e)
        {
            Logger.LogError(e,
                "Evaluation calling exception {method} on {triggerType} (trigger={trigger}) for recipeId={recipeId}, recipeName={recipeName}",
                nameof(ISetupTrigger.Evaluate), trigger.GetType().Name, trigger, recipe?.Id, recipe?.Name);
            throw;
        }
        finally
        {
            Logger.LogTrace("Leaving TryEvaluate (trigger={trigger})", trigger);
        }
    }


    private IReadOnlyList<IWorkplanStep> TryCreateSteps(ISetupTrigger trigger, ProductionRecipe recipe)
    {
        try
        {
            return trigger.CreateSteps(recipe);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Calling {method} on {trigger} threw an exception processing recipe {id}: {name}",
                nameof(ISetupTrigger.CreateSteps), trigger.GetType().Name, recipe.Id, recipe.Name);
            throw;
        }
    }

    /// <summary>
    /// Wire the created steps within the workplan
    /// </summary>
    private static void WireWorkplan(Workplan workplan, IDictionary<int, List<IWorkplanStep>> stepGroups)
    {
        // Create workplan boundaries
        var start = WorkplanInstance.CreateConnector("Start", NodeClassification.Start);
        var end = WorkplanInstance.CreateConnector("End", NodeClassification.End);
        var failed = WorkplanInstance.CreateConnector("Failed", NodeClassification.Failed);
        workplan.Add(start, failed);

        // Wire it all together
        var input = start;
        foreach (var stepGroup in stepGroups.OrderBy(sg => sg.Key))
        {
            var output = stepGroup.Key == stepGroups.Keys.Max() ? end : WorkplanInstance.CreateConnector("Intermediate" + (stepGroup.Key));
            workplan.Add(output);

            var steps = stepGroup.Value;
            if (steps.Count == 1)
            {
                // Default case: Single step for the sort order
                var step = steps[0];
                step.Inputs[0] = input;
                step.Outputs[0] = output;
                step.Outputs[1] = step.Outputs[2] = failed;
            }
            else
            {
                // More than one step requires split and join
                var split = new SplitWorkplanStep(steps.Count);
                split.Inputs[0] = input;
                var join = new JoinWorkplanStep((ushort)steps.Count);
                join.Outputs[0] = output;
                workplan.Add(split, join);

                // All parallel steps are inserted between split and join
                for (int stepIndex = 0; stepIndex < steps.Count; stepIndex++)
                {
                    var stepIn = WorkplanInstance.CreateConnector($"Split-{stepGroup.Key}-{stepIndex + 1}");
                    var stepOut = WorkplanInstance.CreateConnector($"Join-{stepGroup.Key}-{stepIndex + 1}");
                    workplan.Add(stepIn, stepOut);

                    split.Outputs[stepIndex] = stepIn;
                    join.Inputs[stepIndex] = stepOut;

                    var step = steps[stepIndex];
                    step.Inputs[0] = stepIn;
                    step.Outputs[0] = stepOut;
                    step.Outputs[1] = step.Outputs[2] = failed;
                }
            }

            input = output;
        }
    }
}
