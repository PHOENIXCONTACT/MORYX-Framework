// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.Container;
using Moryx.Tools;

namespace Moryx.Products.Management;

/// <summary>
/// Mapper for columns of type <see cref="double"/>
/// </summary>
[FloatStrategyConfiguration]
[Component(LifeCycle.Transient, typeof(IPropertyMapper), Name = nameof(FloatColumnMapper))]
internal class FloatColumnMapper : ColumnMapper<double>
{
    // IEEE 754 special values (NaN, +/-Infinity) are not supported by all databases.
    // PostgreSQL stores them natively in float8, but SQLite throws on REAL columns.
    // To ensure cross-database compatibility, we encode these as sentinel values
    // before writing to the database and decode them back when reading.
    //
    // The sentinels are created using double.BitIncrement, which returns the next
    // representable double value (one ULP — unit of least precision — toward +Infinity).
    // Starting from double.MinValue, each successive BitIncrement
    // produces a distinct, double that is close to double.MinValue.
    // At this extreme, the gap between representable values is heavily minimized, so
    // these values are unlikely to collide with any real-world data.
    //
    // The sentinel values are stored as their raw long bit representation to ensure exact bit-level matching.
    private static readonly double _sentinelNaN = double.BitIncrement(double.MinValue);
    private static readonly double _sentinelPositiveInfinity = double.BitIncrement(_sentinelNaN);
    private static readonly double _sentinelNegativeInfinity = double.BitIncrement(_sentinelPositiveInfinity);

    private static readonly long _sentinelNaNBits = BitConverter.DoubleToInt64Bits(_sentinelNaN);
    private static readonly long _sentinelPositiveInfinityBits = BitConverter.DoubleToInt64Bits(_sentinelPositiveInfinity);
    private static readonly long _sentinelNegativeInfinityBits = BitConverter.DoubleToInt64Bits(_sentinelNegativeInfinity);

    public FloatColumnMapper(Type targetType) : base(targetType)
    {
    }

    /// <inheritdoc />
    protected override IPropertyAccessor<object, double> CreatePropertyAccessor(PropertyInfo objectProp)
    {
        if (objectProp.PropertyType == typeof(double) || objectProp.PropertyType == typeof(float) || objectProp.PropertyType == typeof(decimal))
        {
            return new ConversionAccessor<double, double>(objectProp, ToColumn, ToProperty);
        }

        return base.CreatePropertyAccessor(objectProp);
    }

    private static double ToColumn(double value)
    {
        if (double.IsNaN(value))
        {
            return _sentinelNaN;
        }

        if (double.IsPositiveInfinity(value))
        {
            return _sentinelPositiveInfinity;
        }

        if (double.IsNegativeInfinity(value))
        {
            return _sentinelNegativeInfinity;
        }

        return value;
    }

    private static double ToProperty(double value)
    {
        var bits = BitConverter.DoubleToInt64Bits(value);

        if (bits == _sentinelNaNBits)
        {
            return double.NaN;
        }

        if (bits == _sentinelPositiveInfinityBits)
        {
            return double.PositiveInfinity;
        }

        if (bits == _sentinelNegativeInfinityBits)
        {
            return double.NegativeInfinity;
        }

        return value;
    }
}
