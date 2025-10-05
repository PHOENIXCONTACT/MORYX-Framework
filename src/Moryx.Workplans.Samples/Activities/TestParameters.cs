// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.Serialization;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Moryx.Workplans.Samples.Activities
{
    public class TestParameters : Parameters
    {
        [EntrySerialize(EntrySerializeMode.Never)]
        public int SomeData { get; set; }
                
        public List<string> Items { get; set; }

        protected override void Populate(IProcess process, Parameters instance)
        {
            
        }
    }
}

