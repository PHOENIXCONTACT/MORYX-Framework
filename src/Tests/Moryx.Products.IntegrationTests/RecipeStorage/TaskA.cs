// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.Serialization;

namespace Moryx.Products.IntegrationTests
{
    public class TaskA : TaskStep<ActivityA, ParametersA>
    {
        public override string Name => nameof(TaskA);
    }

    public class ParametersA : Parameters
    {
        [EntrySerialize]
        public int Foo { get; set; }

        protected override void Populate(IProcess process, Parameters instance)
        {
            var parameters = (ParametersA) instance;
            parameters.Foo = Foo;
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
