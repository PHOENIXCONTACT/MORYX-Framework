// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Moryx.Products.Management.Model;

/// <summary>
/// EF value converter that encodes IEEE 754 special values (NaN, +/-Infinity)
/// as sentinel doubles for databases that do not support them natively.
///
/// IEEE 754 special values are not supported by all databases.
/// PostgreSQL stores them natively in float8, but SQLite throws on REAL columns.
/// The converter encodes these as sentinel values before writing to the database
/// and decodes them back when reading.
///
/// The sentinels are created using <see cref="double.BitIncrement(double)"/>, which returns the next
/// representable double value (one ULP — unit of least precision — toward +Infinity).
/// Starting from <see cref="double.MinValue"/>, each successive BitIncrement
/// produces a distinct double that is close to <see cref="double.MinValue"/>.
/// At this extreme, the gap between representable values is heavily minimized, so
/// these values are unlikely to collide with any real-world data.
///
/// The sentinel values are stored as their raw long bit representation to ensure exact bit-level matching.
/// </summary>
internal class Ieee754ValueConverter : ValueConverter<double, double>
{
    private static readonly double _sentinelNaN = double.BitIncrement(double.MinValue);
    private static readonly double _sentinelPositiveInfinity = double.BitIncrement(_sentinelNaN);
    private static readonly double _sentinelNegativeInfinity = double.BitIncrement(_sentinelPositiveInfinity);

    private static readonly long _sentinelNaNBits = BitConverter.DoubleToInt64Bits(_sentinelNaN);
    private static readonly long _sentinelPositiveInfinityBits = BitConverter.DoubleToInt64Bits(_sentinelPositiveInfinity);
    private static readonly long _sentinelNegativeInfinityBits = BitConverter.DoubleToInt64Bits(_sentinelNegativeInfinity);

    public Ieee754ValueConverter() : base(v => ToProvider(v), v => FromProvider(v))
    {
    }

    private static double ToProvider(double value)
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

    private static double FromProvider(double value)
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
