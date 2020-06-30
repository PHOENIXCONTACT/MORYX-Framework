// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Marvin.AbstractionLayer;
using Marvin.AbstractionLayer.Capabilities;

namespace Marvin.Products.IntegrationTests
{
    public class TaskB : TaskStep<ActivityB, ParametersB>
    {
        public override string Name => nameof(TaskB);
    }

    public class ParametersB : ParametersBase
    {
        [EditorBrowsable]
        public SubParameter[] Subs { get; set; }

        protected override ParametersBase ResolveBinding(IProcess process)
        {
            return this;
        }
    }

    public class SubParameter
    {
        public int Type { get; set; }

        public string Name { get; set; }
    }

    [ActivityResults(typeof(DefaultActivityResult))]
    public class ActivityB : Activity<ParametersB>
    {
        public override ProcessRequirement ProcessRequirement { get; }

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
