// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.AbstractionLayer.Products
{
    /// <summary>
    /// Current state of a product
    /// </summary>
    public enum ProductState
    {
        /// <summary>
        /// Object was created, but not reviewed or released for production
        /// </summary>
        Created,

        /// <summary>
        /// Product is released and may be manufactured
        /// </summary>
        Released,

        /// <summary>
        /// Product refers to an old version, that must no longer be manufactured
        /// </summary>
        Deprecated,
    }
}
