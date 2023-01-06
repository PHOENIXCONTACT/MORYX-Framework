// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Workplans;
using System.Linq;

namespace Moryx.AbstractionLayer.TestTools
{
    public class DummyWorkplan : Workplan
    {
        public override bool Equals(object obj)
        {
            var toCompareWith = obj as DummyWorkplan;
            if (toCompareWith == null)
                return false;

            return toCompareWith.Id == Id && toCompareWith.Name == Name 
                && toCompareWith.Version == toCompareWith.Version && toCompareWith.State == State
                && ((toCompareWith.Connectors is null && Connectors is null) || Enumerable.SequenceEqual(toCompareWith.Connectors, Connectors))
                && ((toCompareWith.Steps is null && Steps is null) || Enumerable.SequenceEqual(toCompareWith.Steps, Steps)); 
        }
    }
}
