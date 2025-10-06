// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.Workplans;

namespace Moryx.AbstractionLayer.TestTools
{
    /// <summary>
    /// Dummy implementation of an product instance. Created by the <see cref="DummyProductType"/>
    /// </summary>
    public class DummyProductRecipe : ProductRecipe
    {
        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var toCompareWith = obj as DummyProductRecipe;
            if (toCompareWith == null)
                return false;

            return toCompareWith.Name == Name && toCompareWith.Revision == Revision
                && toCompareWith.State == State && toCompareWith.Classification == Classification
                && ((toCompareWith.Origin is null && Origin is null) || Origin.Equals(toCompareWith.Origin))
                && ((toCompareWith.Product is null && Product is null) || Product.Equals(toCompareWith.Product))
                && ((toCompareWith.Target is null && Target is null) || Target.Equals(toCompareWith.Target));
        }
    }

    /// <summary>
    /// Dummy implementation of a workplan recipe.
    /// </summary>
    public class DummyProductWorkplanRecipe : DummyProductRecipe, IWorkplanRecipe
    {
        /// <inheritdoc/>
        public IWorkplan Workplan { get; set; }

        /// <inheritdoc/>
        public ICollection<long> DisabledSteps { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var toCompareWith = obj as DummyProductWorkplanRecipe;
            if (toCompareWith == null)
                return false;

            return base.Equals(toCompareWith)
                && ((toCompareWith.Workplan is null && Workplan is null) || Workplan.Equals(toCompareWith.Workplan))
                && ((toCompareWith.DisabledSteps is null && DisabledSteps is null) || Enumerable.SequenceEqual<long>(DisabledSteps, toCompareWith.DisabledSteps));
        }
    }
}
