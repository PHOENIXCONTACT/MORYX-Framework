// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Recipes;
using Moryx.AbstractionLayer.Workplans;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.Setups;
using Moryx.Workplans;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Setups
{
    internal abstract class TestTriggerBase : SetupTriggerBase<SetupTriggerConfig>
    {
        public bool RequiredWasCalled { get; protected set; }

        public bool CreateStepCalled { get; protected set; }

        public override IReadOnlyList<IWorkplanStep> CreateSteps(IProductRecipe recipe)
        {
            CreateStepCalled = true;
            return [new TestSetupTask()];
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

    internal class TestSetupTask : TaskStep<TestSetupActivity, TestSetupParameters>, ISetupStep
    {
        public SetupClassification Classification => SetupClassification.Unspecified;
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

        protected override void Populate(Process process, Parameters instance)
        {
            var parameters = (TestSetupParameters)instance;
            parameters.TargetCapabilities = TargetCapabilities;
        }
    }
}
