using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Setups;

namespace Moryx.ControlSystem.SetupProvider.Tests
{
    internal class TestTriggerPrepare : TestTriggerBase
    {
        public override SetupExecution Execution => SetupExecution.BeforeProduction;

        public override SetupEvaluation Evaluate(IProductRecipe recipe)
        {
            RequiredWasCalled = true;

            return SetupEvaluation.Provide(new TestSetupCapabilities { SetupState = ((ITestRecipe)recipe).SetupState });
        }
    }
}
