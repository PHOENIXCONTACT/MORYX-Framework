// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers;

/// <summary>
/// Base API for driver states
/// </summary>
public interface IDriverState
{
    /// <summary>
    /// Gets the classification of the state.
    /// </summary>
    StateClassification Classification { get; }
}