// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Serialization;

internal abstract class NumericStructEntrySerializer : IStructSerializer
{
    public abstract Type TargetType { get; }

    public abstract Entry Encode(object value, IFormatProvider formatProvider);

    public abstract object Decode(Entry entry, IFormatProvider formatProvider);

    protected static Entry CreateFloatEntry(string name, float value, IFormatProvider formatProvider)
    {
        var str = EntryConvert.ConvertToString(value, formatProvider);
        return new Entry
        {
            DisplayName = name,
            Identifier = name,
            Value = new EntryValue
            {
                Type = EntryValueType.Single,
                Current = str,
                Default = EntryConvert.ConvertToString(0f, formatProvider)
            },
            Validation = new EntryValidation
            {
                Minimum = float.MinValue,
                Maximum = float.MaxValue
            }
        };
    }

    protected static float ReadFloat(Entry entry, string name, IFormatProvider formatProvider)
    {
        var sub = entry.SubEntries.Find(e => e.Identifier == name);
        var value = sub?.Value.Current;
        return value != null
            ? (float)EntryConvert.ToObject(typeof(float), value, formatProvider)
            : 0f;
    }
}
