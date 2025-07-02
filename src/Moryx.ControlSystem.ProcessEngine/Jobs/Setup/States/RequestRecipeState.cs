using System.ComponentModel;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.Recipes;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Setup
{
    /// <summary>
    /// In this state a setup job has performed the setup workplan, but something failed so he waits for a new recipe
    /// from the setup manager.
    /// </summary>
    [DisplayName("RetrySetup")]
    internal class RequestRecipeState : SetupJobStateBase
    {
        public override bool RecipeRequired => true;

        public RequestRecipeState(JobDataBase context, StateMap stateMap) : base(context, stateMap, JobClassification.Running)
        {
        }

        public override void Interrupt()
        {
            NextState(StateCompleted);
        }

        public override void Load()
        {
            NextState(StateCompleted);
        }

        public override void UpdateSetup(ISetupRecipe newRecipe)
        {
            if (newRecipe == null)
            {
                NextState(StateCompleted);
            }
            else
            {
                Context.UpdateRecipe(newRecipe);
                NextState(StateRunning);
                Context.StartProcess();
            }
        }

        public override void Abort()
        {
            NextState(StateCompleted);
        }
    }
}