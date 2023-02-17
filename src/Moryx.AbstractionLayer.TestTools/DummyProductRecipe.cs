// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.Workplans;
using System.Collections.Generic;
using System.Linq;

namespace Moryx.AbstractionLayer.TestTools
{
    /// <summary>
    /// Dummy implementation of an product instance. Created by the <see cref="DummyProductType"/>
    /// </summary>
    public class DummyProductRecipe : ProductRecipe
    {
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

    public class DummyProductWorkplanRecipe : DummyProductRecipe, IWorkplanRecipe
    {
        public IWorkplan Workplan { get; set; }

        public ICollection<long> DisabledSteps { get; set; }

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
