// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Serialization;

namespace Moryx.Tests.Serialization;

public class CollectionConverterDummy : EntryEncoderBase
{
    public override bool CanEncode(Type memberType) => memberType == typeof(List<EncoderTypeTargetDummy>);

    public override Entry Encode(object value, EntryEncoderContext context) => new()
    {
        Value = new EntryValue
        {
            Type = EntryValueType.String,
            Current = string.Join(",", ((List<EncoderTypeTargetDummy>)value ?? []).Select(t => t.Tag))
        }
    };

    public override object Decode(Entry entry, object currentValue, EntryEncoderContext context) =>
        entry.Value.Current.Split(',').Select(tag => new EncoderTypeTargetDummy { Tag = tag }).ToList();
}
