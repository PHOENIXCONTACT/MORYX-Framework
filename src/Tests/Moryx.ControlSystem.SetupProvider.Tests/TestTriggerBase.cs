// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.Setups;
using Moryx.Workplans;
using System.Collections.Generic;

namespace Moryx.ControlSystem.SetupProvider.Tests
{
    internal abstract class TestTriggerBase : SetupTriggerBase<SetupTriggerConfig>
    {
        public bool RequiredWasCalled { get; protected set; }

        public bool CreateStepCalled { get; protected set; }

        public override IReadOnlyList<IWorkplanStep> CreateSteps(IProductRecipe recipe)
        {
            CreateStepCalled = true;
            return new[] { new TestSetupTask() };
        }
    }

    internal class TestSetupCapabilities : CapabilitiesBase
    {
        public int SetupState { get; set; }

        protected override bool ProvidedBy(ICapabilities provided)
        {
            return (provided as TestSetupCapabilities)?.SetupState == SetupState;
        }
    }

    internal class TestSetupTask : TaskStep<TestSetupActivity, TestSetupParameters>
    {
    }

    [ActivityResults(typeof(DefaultActivityResult))]
    internal class TestSetupActivity : Activity<TestSetupParameters>, IControlSystemActivity
    {
        public ActivityClassification Classification => ActivityClassification.Production | ActivityClassification.Setup;

        public override ProcessRequirement ProcessRequirement => ProcessRequirement.NotRequired;

        public override ICapabilities RequiredCapabilities => Parameters.TargetCapabilities;


        protected override ActivityResult CreateResult(long resultNumber)
        {
            return ActivityResult.Create((DefaultActivityResult)resultNumber);
        }

        protected override ActivityResult CreateFailureResult()
        {
            return ActivityResult.Create(DefaultActivityResult.TechnicalError);
        }
    }

    internal class TestSetupParameters : Parameters
    {
        public TestSetupCapabilities TargetCapabilities { get; set; }

        protected override void Populate(IProcess process, Parameters instance)
        {
            var parameters = (TestSetupParameters) instance;
            parameters.TargetCapabilities = TargetCapabilities;
        }
    }
}
