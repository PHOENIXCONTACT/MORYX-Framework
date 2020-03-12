// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Marvin.Serialization;
using Marvin.Workflows;
using Marvin.Workflows.Transitions;
using Marvin.Workflows.WorkplanSteps;

namespace Marvin.Tests.Workflows
{
    internal class DefaultValueStep : WorkplanStepBase
    {
        /// 
        public override string Name
        {
            get { return "DefaultValue"; }
        }

        [EditorVisible]
        public int OptionalParameter { get; set; }

        [DefaultValue(10), EditorVisible]
        public ushort OptionalWithDefault { get; set; }

        public DefaultValueStep(int mandatory, ushort mandatoryWithDefault = 2)
        {
            _mandatory = mandatory;
            _mandatoryWithDefault = mandatoryWithDefault;
        }

        ///
        protected override TransitionBase Instantiate(IWorkplanContext context)
        {
            return null;
        }

        private readonly int _mandatory;
        private readonly ushort _mandatoryWithDefault;

    }
}
