// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Serialization;

namespace Moryx.Tests.Serialization;

public abstract class ConverterDummy(string prefix) : EntryEncoderBase
{
    public override bool CanEncode(Type memberType) => memberType == typeof(EncoderTypeTargetDummy);

    public override Entry Encode(object value, EntryEncoderContext context) => new()
    {
        Value = new EntryValue { Type = EntryValueType.String, Current = prefix + (value as EncoderTypeTargetDummy)?.Tag }
    };

    public override object Decode(Entry entry, object currentValue, EntryEncoderContext context) =>
        new EncoderTypeTargetDummy { Tag = entry.Value.Current[prefix.Length..] };
}

public class TypeConverterDummy() : ConverterDummy("type:");

public class PropertyConverterDummy() : ConverterDummy("property:");
