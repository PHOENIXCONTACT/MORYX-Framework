// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.AbstractionLayer.Products;

namespace Moryx.Products.Samples
{
    /// <summary>
    /// Product that represents the package used to ship a watch
    /// </summary>
    [DisplayName("Packaging")]
    public class WatchPackageType : ProductType
    {
        /// <summary>
        /// Watches that can be shipped in this package
        /// </summary>
        [DisplayName("Possible Watches")]
        public List<ProductPartLink<WatchType>> PossibleWatches { get; set; }

        protected override ProductInstance Instantiate()
        {
            return new WatchPackageInstance();
        }
    }
}
