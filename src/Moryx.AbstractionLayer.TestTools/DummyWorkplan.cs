// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Workplans;

namespace Moryx.AbstractionLayer.TestTools
{
    /// <summary>
    /// Dummy implementation of <see cref="IWorkplan"/> with overridden <see cref="Equals(object)"/> method for testing purposes.
    /// </summary>
    public class DummyWorkplan : Workplan
    {
        /// <inheritdoc/>
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
