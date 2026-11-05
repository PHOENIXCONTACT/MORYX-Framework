// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Numerics;

namespace Moryx.Serialization;

internal sealed class Vector3EntryConverter : NumericStructEntryConverter<Vector3>
{
    protected override List<Entry> EncodeComponents(Vector3 value, IFormatProvider formatProvider) =>
    [
        CreateFloatEntry("X", value.X, formatProvider),
        CreateFloatEntry("Y", value.Y, formatProvider),
        CreateFloatEntry("Z", value.Z, formatProvider)
    ];

    protected override Vector3 DecodeComponents(Entry entry, IFormatProvider formatProvider)
    {
        var x = ReadFloat(entry, "X", formatProvider);
        var y = ReadFloat(entry, "Y", formatProvider);
        var z = ReadFloat(entry, "Z", formatProvider);
        return new Vector3(x, y, z);
    }
}
