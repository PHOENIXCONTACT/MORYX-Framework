// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.Serialization;
using Moryx.Workplans;
using Moryx.Workplans.Transitions;
using Moryx.Workplans.WorkplanSteps;

namespace Moryx.Tests.Workplans
{
    internal class DefaultValueStep : WorkplanStepBase
    {
        private readonly int _mandatory;
        private readonly ushort _mandatoryWithDefault;

        [EntrySerialize]
        public int OptionalParameter { get; set; }

        [DefaultValue(10), EntrySerialize]
        public ushort OptionalWithDefault { get; set; }

        public DefaultValueStep(int mandatory, ushort mandatoryWithDefault = 2)
        {
            _mandatory = mandatory;
            _mandatoryWithDefault = mandatoryWithDefault;

            Name = "DefaultValue";
        }

        ///
        protected override TransitionBase Instantiate(IWorkplanContext context)
        {
            return null;
        }

    }
}
