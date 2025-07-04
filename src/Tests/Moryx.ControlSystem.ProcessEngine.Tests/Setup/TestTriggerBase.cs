﻿using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.Setups;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Setup
{
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

        protected override void Populate(IProcess process, Parameters instance)
        {
            var parameters = (TestSetupParameters) instance;
            parameters.TargetCapabilities = TargetCapabilities;
        }
    }
}