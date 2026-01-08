// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.PickByLight;

/// <summary>
/// Mode of the light of the pick-by-light system
/// </summary>
public enum LightMode
{
    /// <summary>
    /// Light is turned off
    /// </summary>
    Off,

    /// <summary>
    /// Light is solid
    /// </summary>
    Solid,

    /// <summary>
    /// Light blinks slow
    /// </summary>
    BlinkSlow,

    /// <summary>
    /// Light blinks fast
    /// </summary>
    BlinkFast,

    /// <summary>
    /// All lights should be off
    /// </summary>
    AllOff
}
