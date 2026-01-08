// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.PickByLight;

/// <summary>
/// Instruction for the pick-by-light system
/// </summary>
public class LightInstruction
{
    /// <summary>
    /// The amount of elements to pick
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    /// Instruction text
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// The color (if led present)
    /// </summary>
    public int Color { get; set; }

    /// <summary>
    /// The light blinkMode <see cref="PickByLight.LightMode"/>
    /// </summary>
    public LightMode LightMode { get; set; } = LightMode.Solid;
}
