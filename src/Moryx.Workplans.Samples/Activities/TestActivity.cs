// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Capabilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moryx.Workplans.Samples.Activities
{
    [ActivityResults(typeof(DefaultActivityResult))]
    public class TestActivity : Activity<TestParameters, Tracing>
    {
        public override ProcessRequirement ProcessRequirement => ProcessRequirement.Required;

        public override ICapabilities RequiredCapabilities => throw new NotImplementedException();

        protected override ActivityResult CreateFailureResult()
        {
            throw new NotImplementedException();
        }

        protected override ActivityResult CreateResult(long resultNumber)
        {
            throw new NotImplementedException();
        }
    }
}

