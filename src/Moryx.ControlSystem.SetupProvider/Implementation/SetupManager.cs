using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.ControlSystem.Recipes;
using Moryx.ControlSystem.Setups;
using Moryx.Logging;
using Moryx.Workplans;
using Moryx.Workplans.WorkplanSteps;

namespace Moryx.ControlSystem.SetupProvider
{
    [Component(LifeCycle.Singleton, typeof(ISetupManager))]
    internal class SetupManager : ISetupManager, ILoggingComponent
    {
        private readonly ICollection<ISetupTrigger> _triggers = new List<ISetupTrigger>();

        #region Dependencies

        /// <summary>
        /// Logger for this component
        /// </summary>
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

        public ISetupRecipe RequiredSetup(SetupExecution execution, IProductionRecipe recipe, ISetupTarget targetSystem)
        {
            // Determine the triggers
            var triggers = new List<ISetupTrigger>();
            // TODO: This loop restores old behavior with the new API, but it has much more potential
            foreach (var trigger in _triggers.Where(t => t.Execution == execution))
            {
                var evaluation = TryEvaluate(trigger, recipe);

                // No action => skip
                if (!evaluation.Required)
                    continue;

                // Skip the trigger if its capabilities are already provided
                if (execution == SetupExecution.BeforeProduction &&
                    evaluation is SetupEvaluation.Change provide &&
                    targetSystem.Cells(provide.TargetCapabilities).Any())
                    continue;

                // Skip the trigger, if the setup wants to remove capabilities that are not present
                if (execution == SetupExecution.AfterProduction &&
                    evaluation is SetupEvaluation.Change remove &&
                    !targetSystem.Cells(remove.CurrentCapabilities).Any())
                    continue;

                triggers.Add(trigger);
            }
            // Create all necessary setup steps
            var stepGroups = new Dictionary<int, List<IWorkplanStep>>();
            foreach (var trigger in triggers.OrderBy(t => t.SortOrder))
            {
                // Create step collection for first entry of a sort order
                if (!stepGroups.ContainsKey(trigger.SortOrder))
                    stepGroups[trigger.SortOrder] = new List<IWorkplanStep>();

                // Check if extended interface is implemented
                stepGroups[trigger.SortOrder].AddRange(TryCreateSteps(trigger, recipe));
            }
            if (stepGroups.Count == 0) // No setup necessary for this recipe
                return null;

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

        private SetupEvaluation TryEvaluate(ISetupTrigger trigger, IProductionRecipe recipe)
        {
            try
            {
                return trigger.Evaluate(recipe);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Calling {method} on {trigger} threw an exception prcessing recipe {id}: {name}",
                    nameof(ISetupTrigger.Evaluate), trigger.GetType().Name, recipe.Id, recipe.Name);
                throw;
            }
        }

        private IReadOnlyList<IWorkplanStep> TryCreateSteps(ISetupTrigger trigger, IProductionRecipe recipe)
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
            foreach(var stepGroup in stepGroups.OrderBy(sg => sg.Key))
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
}