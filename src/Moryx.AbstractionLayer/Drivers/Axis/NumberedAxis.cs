// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Axis;

/// <summary>
/// A system might have more than one axes of the same name
/// </summary>
public struct NumberedAxis : IEquatable<NumberedAxis>
{
    /// <summary>
    /// Axis
    /// </summary>
    public Axes Axis { get; }

    /// <summary>
    /// Number
    /// </summary>
    public int Number { get; }

    /// <summary>
    /// Create a new instance of <see cref="NumberedAxis"/>
    /// </summary>
    public NumberedAxis(Axes axis, int number)
    {
        Axis = axis;
        Number = number;
    }

    /// <summary>
    /// String representation of the axes
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{Axis}{Number}";
    }

    /// <summary>
    /// Compare two <see cref="NumberedAxis"/>
    /// </summary>
    public bool Equals(NumberedAxis other)
    {
        return Axis == other.Axis && Number == other.Number;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        return obj is NumberedAxis && Equals((NumberedAxis)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            return ((int)Axis * 397) ^ Number;
        }
    }
}