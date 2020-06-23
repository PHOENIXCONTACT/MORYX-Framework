// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Workflows;
using Moryx.Workflows.Transitions;
using Moryx.Workflows.WorkplanSteps;

namespace Moryx.Tests.Workflows
{
    public class InvalidStep : WorkplanStepBase
    {
        /// 
        public override string Name
        {
            get { return "Invalid"; }
        }

        private InvalidStep()
        {
            
        }

        ///
        protected override TransitionBase Instantiate(IWorkplanContext context)
        {
            return new DummyTransition();
        }
    }
}
