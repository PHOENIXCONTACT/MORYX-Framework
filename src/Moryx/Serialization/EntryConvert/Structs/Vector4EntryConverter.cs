// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Numerics;

namespace Moryx.Serialization;

internal sealed class Vector4EntryConverter : NumericStructEntryConverter<Vector4>
{
    protected override List<Entry> EncodeComponents(Vector4 value, IFormatProvider formatProvider) =>
    [
        CreateFloatEntry("X", value.X, formatProvider),
        CreateFloatEntry("Y", value.Y, formatProvider),
        CreateFloatEntry("Z", value.Z, formatProvider),
        CreateFloatEntry("W", value.W, formatProvider)
    ];

    protected override Vector4 DecodeComponents(Entry entry, IFormatProvider formatProvider)
    {
        var x = ReadFloat(entry, "X", formatProvider);
        var y = ReadFloat(entry, "Y", formatProvider);
        var z = ReadFloat(entry, "Z", formatProvider);
        var w = ReadFloat(entry, "W", formatProvider);
        return new Vector4(x, y, z, w);
    }
}
