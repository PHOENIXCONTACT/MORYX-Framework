// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Products;

/// <summary>
/// The state of an <see cref="ProductInstance"/>.
/// This will not occupy more than 4 Bits. Other enums can be added by bit-shifting
/// </summary>
[Flags]
public enum ProductInstanceState : int
{
    /// <summary>
    /// Initial state
    /// </summary>
    Unset = 1 << 0,

    /// <summary>
    /// The instance is currently in production
    /// </summary>
    InProduction = 1 << 1,

    /// <summary>
    /// The production on this instance was paused
    /// </summary>
    Paused = 1 << 2,

    /// <summary>
    /// The production process succeeded.
    /// </summary>
    Success = 1 << 3,

    /// <summary>
    /// The production process failed.
    /// </summary>
    Failure = 1 << 4,
}