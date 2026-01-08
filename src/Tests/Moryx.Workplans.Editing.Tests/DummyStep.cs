// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Serialization;
using Moryx.Workplans;
using Moryx.Workplans.Transitions;
using Moryx.Workplans.WorkplanSteps;

namespace Moryx.Tests.Workplans
{
    /// <summary>
    /// DummyWorkplanSteps
    /// </summary>
    [DataContract]
    public class DummyStep : WorkplanStepBase
    {
        private DummyStep()
        {

        }

        public DummyStep(int outputs)
            : this(outputs, "DummyStep")
        {
        }

        public DummyStep(int outputs, string name)
        {
            Outputs = new IConnector[outputs];
            Name = name;
        }

        [EntrySerialize]
        public int Number { get; set; }

        ///
        protected override TransitionBase Instantiate(IWorkplanContext context)
        {
            return new DummyTransition { Context = context, Name = Name };
        }
    }
}