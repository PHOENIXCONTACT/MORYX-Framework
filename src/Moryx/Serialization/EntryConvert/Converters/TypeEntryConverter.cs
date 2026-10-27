// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Tools;

namespace Moryx.Serialization;

/// <summary>
/// Built-in <see cref="EntryConverter"/> for members of type <see cref="Type"/>.
/// Types are encoded as their name. Decoding is restricted to the types of a <see cref="PossibleTypesAttribute"/>,
/// members without the attribute are read-only.
/// </summary>
/// <remarks>
/// The limitation by and coupling to <see cref="PossibleTypesAttribute"/> allows to prevent arbitrary
/// <see cref="Type"/>s to be resolvable from an externally created <see cref="Entry"/>
/// </remarks>
internal sealed class TypeEntryConverter : EntryConverter
{
    /// <inheritdoc />
    public override bool CanConvert(Type memberType) => typeof(Type).IsAssignableFrom(memberType);

    /// <inheritdoc />
    public override Entry Encode(object value, EntryConverterContext context)
    {
        var type = value as Type;
        var possible = context.Serialization.PossibleValues(context.MemberType, context.AttributeProvider, context.Instance);

        string current;
        bool isReadOnly;
        if (possible is { Length: > 0 })
        {
            current = possible.FirstOrDefault(p => p.Key == type?.FullName || p.Key == type?.Name)?.Key;
            isReadOnly = false;
        }
        else
        {
            current = type?.FullName;
            isReadOnly = true;
        }

        return new Entry
        {
            Value = new EntryValue
            {
                Type = EntryValueType.String,
                Current = current,
                Possible = possible,
                IsReadOnly = isReadOnly
            }
        };
    }

    /// <inheritdoc />
    public override object Decode(Entry entry, object currentValue, EntryConverterContext context)
    {
        var attribute = context.AttributeProvider.GetCustomAttribute<PossibleTypesAttribute>();
        if (attribute is null)
        {
            return currentValue;
        }

        var key = entry.Value.Current;
        if (string.IsNullOrEmpty(key))
        {
            return null;
        }

        return attribute.Parse(null, null, key)
            ?? throw new ArgumentException($"Invalid type {key} for entry {entry.DisplayName ?? entry.Identifier}.");
    }
}
