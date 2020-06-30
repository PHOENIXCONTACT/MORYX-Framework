// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;

namespace Moryx.Products.Samples
{
    /// <summary>
    /// Instance of a single cardboard package
    /// </summary>
    public class WatchPackageInstance : ProductInstance<WatchPackageType>
    {
        /// <summary>
        /// Reference to the watch packed in this box
        /// </summary>
        public WatchInstance PackedWatch { get; set; }
    }
}
