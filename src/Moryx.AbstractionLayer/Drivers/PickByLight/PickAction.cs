// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.PickByLight;

/// <summary>
/// Pick-action if supported by pick-by-light system
/// </summary>
public enum PickAction
{
    /// <summary>
    /// No action required
    /// </summary>
    Empty,

    /// <summary>
    /// Pick element from the position
    /// </summary>
    Pick,

    /// <summary>
    /// Additional information for the user
    /// </summary>
    Info
}
