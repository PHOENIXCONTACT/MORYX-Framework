// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Numerics;

namespace Moryx.Serialization;

internal sealed class Vector2EntryConverter : NumericStructEntryConverter<Vector2>
{
    protected override List<Entry> EncodeComponents(Vector2 value, IFormatProvider formatProvider) =>
    [
        CreateFloatEntry("X", value.X, formatProvider),
        CreateFloatEntry("Y", value.Y, formatProvider)
    ];

    protected override Vector2 DecodeComponents(Entry entry, IFormatProvider formatProvider)
    {
        var x = ReadFloat(entry, "X", formatProvider);
        var y = ReadFloat(entry, "Y", formatProvider);
        return new Vector2(x, y);
    }
}
