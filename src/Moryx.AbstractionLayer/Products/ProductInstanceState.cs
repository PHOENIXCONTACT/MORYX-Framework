// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Products
{
    /// <summary>
    /// The state of an Article. This will not occupy more than 4 Bits. Other enums can be added by bit-shifting
    /// </summary>
    public enum ProductInstanceState : byte
    {
        /// <summary>
        /// Initial state
        /// </summary>
        Unset,

        /// <summary>
        /// The instance is currently in production
        /// </summary>
        InProduction,

        /// <summary>
        /// The production on this instance was paused
        /// </summary>
        Paused,

        /// <summary>
        /// The production process succeeded.
        /// </summary>
        Success,

        /// <summary>
        /// The production process failed.
        /// </summary>
        Failure
    }
}
