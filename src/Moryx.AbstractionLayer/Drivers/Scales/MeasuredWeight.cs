// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Diagnostics;

namespace Moryx.AbstractionLayer.Drivers.Scales;

/// <summary>
/// Result weight scales
/// </summary>
[DebuggerDisplay("Weight = {Weight}, Unit = {Unit}")]
public class MeasuredWeight
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MeasuredWeight"/> class.
    /// </summary>
    public MeasuredWeight(double weight, string unit)
    {
        Weight = weight;
        Unit = unit;
    }

    /// <summary>
    /// The read weight
    /// </summary>
    public double Weight { get; }

    /// <summary>
    /// Gets or sets the unit.
    /// </summary>
    public string Unit { get; }
}
