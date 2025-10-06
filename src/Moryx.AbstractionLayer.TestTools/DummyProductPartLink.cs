// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;

namespace Moryx.AbstractionLayer.TestTools
{
    /// <summary>
    /// Dummy implementation of a ProductPartLink.
    /// </summary>
    public class DummyProductPartLink : ProductPartLink<DummyProductType>
    {
        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var toCompareWith = obj as DummyProductPartLink;
            if (toCompareWith == null)
                return false;

            return GetType().GetProperties()
                .All(prop => (prop.GetValue(toCompareWith) is null && prop.GetValue(this) is null)
                            || prop.GetValue(toCompareWith).Equals(prop.GetValue(this)));
        }
    }
}
