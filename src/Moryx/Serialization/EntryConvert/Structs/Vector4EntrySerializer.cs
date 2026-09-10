// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Numerics;

namespace Moryx.Serialization;

internal class Vector4EntrySerializer : NumericStructEntrySerializer
{
    public override Type TargetType => typeof(Vector4);

    public override Entry Encode(object value, IFormatProvider formatProvider)
    {
        var v = (Vector4)value;
        return new Entry
        {
            SubEntries =
            [
                CreateFloatEntry("X", v.X, formatProvider),
                CreateFloatEntry("Y", v.Y, formatProvider),
                CreateFloatEntry("Z", v.Z, formatProvider),
                CreateFloatEntry("W", v.W, formatProvider)
            ]
        };
    }

    public override object Decode(Entry entry, IFormatProvider formatProvider)
    {
        var x = ReadFloat(entry, "X", formatProvider);
        var y = ReadFloat(entry, "Y", formatProvider);
        var z = ReadFloat(entry, "Z", formatProvider);
        var w = ReadFloat(entry, "W", formatProvider);
        return new Vector4(x, y, z, w);
    }
}
