// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Numerics;

namespace Moryx.Serialization;

internal sealed class QuaternionEntryConverter : NumericStructEntryConverter<Quaternion>
{
    protected override List<Entry> EncodeComponents(Quaternion value, IFormatProvider formatProvider) =>
    [
        CreateFloatEntry("X", value.X, formatProvider),
        CreateFloatEntry("Y", value.Y, formatProvider),
        CreateFloatEntry("Z", value.Z, formatProvider),
        CreateFloatEntry("W", value.W, formatProvider)
    ];

    protected override Quaternion DecodeComponents(Entry entry, IFormatProvider formatProvider)
    {
        var x = ReadFloat(entry, "X", formatProvider);
        var y = ReadFloat(entry, "Y", formatProvider);
        var z = ReadFloat(entry, "Z", formatProvider);
        var w = ReadFloat(entry, "W", formatProvider);
        return new Quaternion(x, y, z, w);
    }
}
