// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Numerics;

namespace Moryx.Serialization;

internal class QuaternionEntrySerializer : NumericStructEntrySerializer
{
    public override Type TargetType => typeof(Quaternion);

    public override Entry Encode(object value, IFormatProvider formatProvider)
    {
        var q = (Quaternion)value;
        return new Entry
        {
            SubEntries =
            [
                CreateFloatEntry("X", q.X, formatProvider),
                CreateFloatEntry("Y", q.Y, formatProvider),
                CreateFloatEntry("Z", q.Z, formatProvider),
                CreateFloatEntry("W", q.W, formatProvider)
            ]
        };
    }

    public override object Decode(Entry entry, IFormatProvider formatProvider)
    {
        var x = ReadFloat(entry, "X", formatProvider);
        var y = ReadFloat(entry, "Y", formatProvider);
        var z = ReadFloat(entry, "Z", formatProvider);
        var w = ReadFloat(entry, "W", formatProvider);
        return new Quaternion(x, y, z, w);
    }
}
