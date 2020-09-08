// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.Serialization;
using Moryx.Workflows;
using Moryx.Workflows.Transitions;
using Moryx.Workflows.WorkplanSteps;

namespace Moryx.Tests.Workflows
{
    public class EmptyParameters
    {
        public int Hidden { get; set; }
    }

    public class EmptyParametersStep : WorkplanStepBase
    {
        ///
        public override string Name => "EmptyParameters";

        [EntrySerialize]
        public EmptyParameters Parameters { get; set; }

        ///
        protected override TransitionBase Instantiate(IWorkplanContext context)
        {
            return null;
        }
    }
}
