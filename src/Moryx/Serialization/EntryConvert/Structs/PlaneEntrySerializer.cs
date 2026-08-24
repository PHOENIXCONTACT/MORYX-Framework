// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Numerics;

namespace Moryx.Serialization;

internal class PlaneEntrySerializer : NumericStructEntrySerializer
{
    public override Type TargetType => typeof(Plane);

    public override Entry Encode(object value, IFormatProvider formatProvider)
    {
        var p = (Plane)value;
        return new Entry
        {
            SubEntries =
            [
                CreateFloatEntry("X", p.Normal.X, formatProvider),
                CreateFloatEntry("Y", p.Normal.Y, formatProvider),
                CreateFloatEntry("Z", p.Normal.Z, formatProvider),
                CreateFloatEntry("D", p.D, formatProvider)
            ]
        };
    }

    public override object Decode(Entry entry, IFormatProvider formatProvider)
    {
        var x = ReadFloat(entry, "X", formatProvider);
        var y = ReadFloat(entry, "Y", formatProvider);
        var z = ReadFloat(entry, "Z", formatProvider);
        var d = ReadFloat(entry, "D", formatProvider);
        return new Plane(new Vector3(x, y, z), d);
    }
}
