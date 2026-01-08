// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;
using Moryx.Serialization;

namespace Moryx.Workplans.Samples.Activities
{
    public class TestParameters : Parameters
    {
        [EntrySerialize(EntrySerializeMode.Never)]
        public int SomeData { get; set; }

        public List<string> Items { get; set; }

        protected override void Populate(Process process, Parameters instance)
        {

        }
    }
}

