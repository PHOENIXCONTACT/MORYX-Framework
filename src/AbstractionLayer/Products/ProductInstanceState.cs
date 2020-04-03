// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.AbstractionLayer.Products
{
    /// <summary>
    /// The state of an Article. This will not occupy more than 4 Bits. Other enums can be added by bit-shifting
    /// </summary>
    public enum ProductInstanceState : byte
    {
        /// <summary>
        /// Initial state
        /// </summary>
        Initial = 0,

        /// <summary>
        /// The instance is currently in production
        /// </summary>
        InProduction = 1,

        /// <summary>
        /// The production on this instance was paused
        /// </summary>
        Paused = 2,

        /// <summary>
        /// The production process succeeded.
        /// </summary>
        Success = 3,

        /// <summary>
        /// The production process failed.
        /// </summary>
        Failure = 4,

        /// <summary>
        /// State of part is inherited from the parent instance
        /// </summary>
        Inherited = 5
    }
}
