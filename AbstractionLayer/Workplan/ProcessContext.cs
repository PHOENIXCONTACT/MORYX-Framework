using Marvin.Workflows;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Context for workplans that execute a process
    /// </summary>
    public struct ProcessContext : IWorkplanContext
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

        /// <summary>
        /// Check if a step was disabled
        /// </summary>
        public bool IsDisabled(long stepId)
        {
            return ((IWorkplanRecipe)Process.Recipe).DisabledSteps.Contains(stepId);
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