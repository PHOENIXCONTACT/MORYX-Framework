// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.AbstractionLayer.Recipes;
using Marvin.Workflows;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Context for workplans that execute a process
    /// </summary>
    public class ProcessContext : IWorkplanContext
    {
        /// <summary>
        /// Create process context for process
        /// </summary>
        public ProcessContext(IProcess process)
        {
            Process = process;
        }

        /// <summary>
        /// Process the workplan is executed on
        /// </summary>
        public IProcess Process { get; }

        /// <inheritdoc />
        public virtual bool IsDisabled(IWorkplanStep step)
        {
            return ((IWorkplanRecipe)Process.Recipe).DisabledSteps.Contains(step.Id);
        }

        /// <summary>
        /// Find a preassigned resource id
        /// </summary>
        public long PreassignedResource(long taskId)
        {
            var recipe = (IWorkplanRecipe) Process.Recipe;
            return recipe.TaskAssignment.ContainsKey(taskId) ? recipe.TaskAssignment[taskId] : 0;
        }
    }
}
