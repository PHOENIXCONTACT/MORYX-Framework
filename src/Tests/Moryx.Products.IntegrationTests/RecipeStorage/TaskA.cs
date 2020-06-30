// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.Products.IntegrationTests
{
    public class TaskA : TaskStep<ActivityA, ParametersA>
    {
        public override string Name => nameof(TaskA);
    }

    public class ParametersA : ParametersBase
    {
        [EditorBrowsable]
        public int Foo { get; set; }

        protected override ParametersBase ResolveBinding(IProcess process)
        {
            return this;
        }
    }

    [ActivityResults(typeof(DefaultActivityResult))]
    public class ActivityA : Activity<ParametersA>
    {
        public override ProcessRequirement ProcessRequirement => ProcessRequirement.NotRequired;

        public override ICapabilities RequiredCapabilities { get; }

        protected override ActivityResult CreateResult(long resultNumber)
        {
            throw new System.NotImplementedException();
        }

        protected override ActivityResult CreateFailureResult()
        {
            throw new System.NotImplementedException();
        }
    }
}
