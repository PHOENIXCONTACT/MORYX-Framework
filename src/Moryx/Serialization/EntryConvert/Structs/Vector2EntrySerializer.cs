// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Numerics;

namespace Moryx.Serialization;

internal class Vector2EntrySerializer : NumericStructEntrySerializer
{
    public override Type TargetType => typeof(Vector2);

    public override Entry Encode(object value, IFormatProvider formatProvider)
    {
        var v = (Vector2)value;
        return new Entry
        {
            SubEntries =
            [
                CreateFloatEntry("X", v.X, formatProvider),
                CreateFloatEntry("Y", v.Y, formatProvider)
            ]
        };
    }

    public override object Decode(Entry entry, IFormatProvider formatProvider)
    {
        var x = ReadFloat(entry, "X", formatProvider);
        var y = ReadFloat(entry, "Y", formatProvider);
        return new Vector2(x, y);
    }
}
