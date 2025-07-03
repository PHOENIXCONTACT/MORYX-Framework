using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Setups;
using Moryx.Workplans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moryx.ControlSystem.SetupProvider.Tests
{
    internal class MultiStepTrigger : TestTriggerBase
    {
        public override SetupExecution Execution => SetupExecution.BeforeProduction;

        public override SetupEvaluation Evaluate(IProductRecipe recipe)
        {
            RequiredWasCalled = true;

            return SetupEvaluation.Provide(new TestSetupCapabilities { SetupState = ((ITestRecipe)recipe).SetupState });
        }

        public override IReadOnlyList<IWorkplanStep> CreateSteps(IProductRecipe recipe)
        {
            CreateStepCalled = true;
            return new IWorkplanStep[] { new TestSetupTask { Name = "P1" }, new TestSetupTask { Name = "P2" } };
        } 
    }
}
