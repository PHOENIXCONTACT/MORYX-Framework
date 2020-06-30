// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Marvin.Workflows;
using Marvin.Workflows.Transitions;
using Marvin.Workflows.WorkplanSteps;

namespace Marvin.Tests.Workflows
{
    public class EmptyParameters
    {
        public int Hidden { get; set; } 
    }

    public class EmptyParametersStep : WorkplanStepBase
    {
        /// 
        public override string Name
        {
            get { return "EmptyParameters"; }
        }

        [EditorBrowsable]
        public EmptyParameters Parameters { get; set; }

        ///
        protected override TransitionBase Instantiate(IWorkplanContext context)
        {
            return null;
        }
    }
}
