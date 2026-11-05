// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Numerics;

namespace Moryx.Serialization;

internal sealed class PlaneEntryConverter : NumericStructEntryConverter<Plane>
{
    protected override List<Entry> EncodeComponents(Plane value, IFormatProvider formatProvider) =>
    [
        CreateFloatEntry("X", value.Normal.X, formatProvider),
        CreateFloatEntry("Y", value.Normal.Y, formatProvider),
        CreateFloatEntry("Z", value.Normal.Z, formatProvider),
        CreateFloatEntry("D", value.D, formatProvider)
    ];

    protected override Plane DecodeComponents(Entry entry, IFormatProvider formatProvider)
    {
        var x = ReadFloat(entry, "X", formatProvider);
        var y = ReadFloat(entry, "Y", formatProvider);
        var z = ReadFloat(entry, "Z", formatProvider);
        var d = ReadFloat(entry, "D", formatProvider);
        return new Plane(new Vector3(x, y, z), d);
    }
}
