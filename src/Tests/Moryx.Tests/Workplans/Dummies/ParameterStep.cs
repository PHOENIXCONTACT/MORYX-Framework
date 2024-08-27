// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Serialization;
using Moryx.Workplans;
using Moryx.Workplans.Transitions;
using Moryx.Workplans.WorkplanSteps;

namespace Moryx.Tests.Workplans
{
    internal class ParameterStep : WorkplanStepBase, IParameterHolder
    {
        public ParameterStep()
        {
            Name = "ParameterStep";
        }

        [EntrySerialize]
        public DummyParameters Parameters { get; set; }

        ///
        protected override TransitionBase Instantiate(IWorkplanContext context)
        {
            return new DummyTransition();
        }

        public DummyParameters Export()
        {
            return Parameters;
        }
    }

    internal class ParameterConstructorStep : WorkplanStepBase, IParameterHolder
    {
        private readonly DummyParameters _parameters;

        public ParameterConstructorStep(DummyParameters parameters)
        {
            _parameters = parameters;
            Name = "ParameterConstructorStep";
        }

        ///
        protected override TransitionBase Instantiate(IWorkplanContext context)
        {
            return new DummyTransition();
        }

        public DummyParameters Export()
        {
            return _parameters;
        }
    }

    internal interface IParameterHolder
    {
        DummyParameters Export();
    }

    internal class DummyParameters
    {
        [EntrySerialize]
        public int Number { get; set; }

        [EntrySerialize]
        public string Name { get; set; }
    }
}
