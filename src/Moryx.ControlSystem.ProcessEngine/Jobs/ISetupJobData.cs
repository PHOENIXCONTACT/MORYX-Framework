using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.Recipes;

namespace Moryx.ControlSystem.ProcessEngine.Jobs
{
    /// <summary>
    /// Public representation of a setup job
    /// </summary>
    internal interface ISetupJobData : IJobData
    {
        /// <summary>
        /// Current recipe of this job
        /// </summary>
        new ISetupRecipe Recipe { get; }

        /// <summary>
        /// Number of running setup activities
        /// </summary>
        int RunningCount { get; }

        /// <summary>
        /// Number of completed setup activities
        /// </summary>
        int CompletedCount { get; }

        /// <summary>
        /// Indicator that the setup needs an up to date recipe
        /// </summary>
        bool RecipeRequired { get; }

        /// <summary>
        /// The process instance that executes the setup recipe
        /// </summary>
        ProcessData ActiveProcess { get; }

        /// <summary>
        /// The job failed to properly setup the machine. 
        /// It must retry with a new recipe that should make the necessary changes.
        /// </summary>
        void UpdateSetup(ISetupRecipe updatedRecipe);
    }
}