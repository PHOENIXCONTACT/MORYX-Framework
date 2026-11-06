// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Serialization;

/// <summary>
/// Base class for built-in encoders of numeric structs, which are decomposed into float sub-entries
/// </summary>
internal abstract class NumericStructEntryEncoder : EntryEncoderBase
{
    /// <summary>
    /// The struct type this encoder handles
    /// </summary>
    public abstract Type TargetType { get; }

    /// <inheritdoc />
    public override bool CanEncode(Type memberType) => memberType == TargetType;

    /// <summary>
    /// Create a sub-entry for a single float component
    /// </summary>
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
            }
        };
    }

    /// <summary>
    /// Read a single float component from the sub-entries
    /// </summary>
    protected static float ReadFloat(Entry entry, string name, IFormatProvider formatProvider)
    {
        var sub = entry.SubEntries.Find(e => e.Identifier == name);
        var value = sub?.Value.Current;
        return value != null
            ? (float)EntryConvert.ToObject(typeof(float), value, formatProvider)
            : 0f;
    }
}

/// <summary>
/// Typed base class for built-in encoders of numeric structs
/// </summary>
internal abstract class NumericStructEntryEncoder<TStruct> : NumericStructEntryEncoder
    where TStruct : struct
{
    /// <inheritdoc />
    public override Type TargetType => typeof(TStruct);

    /// <inheritdoc />
    public override Entry Encode(object value, EntryEncoderContext context)
    {
        var hasValue = value is TStruct;
        var typedValue = hasValue ? (TStruct)value : default;
        var subEntries = EncodeComponents(typedValue, context.Serialization.FormatProvider);

        // Only set display name for actual values; prototypes pass null and should fall back to the member metadata
        var displayName = hasValue ? string.Join(", ", subEntries.Select(e => $"{e.Identifier}={e.Value.Current}")) : null;

        var typeName = typeof(TStruct).Name;

        return new Entry
        {
            DisplayName = displayName,
            Value = new EntryValue
            {
                Type = EntryValueType.Struct,
                Current = typeName,
                Default = typeName
            },
            SubEntries = subEntries
        };
    }

    /// <inheritdoc />
    public override object Decode(Entry entry, object currentValue, EntryEncoderContext context) =>
        DecodeComponents(entry, context.Serialization.FormatProvider);

    /// <summary>
    /// Encode the components of the struct into sub-entries
    /// </summary>
    protected abstract List<Entry> EncodeComponents(TStruct value, IFormatProvider formatProvider);

    /// <summary>
    /// Decode the struct from the component sub-entries
    /// </summary>
    protected abstract TStruct DecodeComponents(Entry entry, IFormatProvider formatProvider);
}
