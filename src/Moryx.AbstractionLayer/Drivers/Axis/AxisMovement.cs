// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Axis;

/// <summary>
/// Defines a axis movement
/// </summary>
public class AxisMovement
{
    /// <summary>
    /// The axis which should be moved
    /// </summary>
    public Axes Axis { get; }

    /// <summary>
    /// Target position of the axis to move to
    /// </summary>
    public double? TargetPosition { get; }

    /// <summary>
    /// Predefined position of the axis to move to
    /// </summary>
    public AxisPosition? PredefinedPosition { get; }

    /// <summary>
    /// Creates a new instance of <see cref="AxisMovement"/> with a target position
    /// </summary>
    /// <param name="axis">The axis which should be moved</param>
    /// <param name="targetPosition">Target position of the axis to move to</param>
    public AxisMovement(Axes axis, double targetPosition)
    {
        Axis = axis;
        TargetPosition = targetPosition;
    }

    /// <summary>
    /// Creates a new instance of <see cref="AxisMovement"/> with a predefined position
    /// </summary>
    /// <param name="axis">The axis which should be moved</param>
    /// <param name="predefinedPosition">Predefined position of the axis to move to</param>
    public AxisMovement(Axes axis, AxisPosition predefinedPosition)
    {
        Axis = axis;
        PredefinedPosition = predefinedPosition;
    }
}
