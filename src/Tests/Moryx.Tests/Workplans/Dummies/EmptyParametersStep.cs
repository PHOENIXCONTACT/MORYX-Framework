// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Serialization;
using Moryx.Workplans;
using Moryx.Workplans.Transitions;
using Moryx.Workplans.WorkplanSteps;

namespace Moryx.Tests.Workplans
{
    public class EmptyParameters
    {
        public int Hidden { get; set; }
    }

    public class EmptyParametersStep : WorkplanStepBase
    {
        public EmptyParametersStep()
        {
            Name = "EmptyParameters";
        }

        [EntrySerialize]
        public EmptyParameters Parameters { get; set; }

        ///
        protected override TransitionBase Instantiate(IWorkplanContext context)
        {
            return null;
        }
    }
}
