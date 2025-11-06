// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.Simulation.Tests
{
    [ActivityResults(typeof(DefaultActivityResult))]
    public class AssemblyActivity : Activity<NullActivityParameters>
    {
        public override ProcessRequirement ProcessRequirement => ProcessRequirement.Required;

        public override ICapabilities RequiredCapabilities => new AssemblyCapabilities();

        protected override ActivityResult CreateFailureResult()
        {
            return ActivityResult.Create(DefaultActivityResult.Failed);
        }

        protected override ActivityResult CreateResult(long resultNumber)
        {
            return ActivityResult.Create((DefaultActivityResult)resultNumber);
        }
    }
}

