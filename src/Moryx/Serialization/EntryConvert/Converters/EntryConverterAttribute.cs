// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Serialization;

/// <summary>
/// Defines the <see cref="EntryConverter"/> used to convert a member or all members of a type.
/// A converter defined on a member takes precedence over a converter defined on the member's type.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
public sealed class EntryConverterAttribute : Attribute
{
    /// <summary>
    /// Create a new instance of <see cref="EntryConverterAttribute"/>
    /// </summary>
    /// <param name="converterType">Type deriving from <see cref="EntryConverter"/> with a public parameterless constructor</param>
    public EntryConverterAttribute(Type converterType)
    {
        ArgumentNullException.ThrowIfNull(converterType);

        if (!typeof(EntryConverter).IsAssignableFrom(converterType) || converterType.IsAbstract)
        {
            throw new ArgumentException($"{converterType.Name} is not a non-abstract {nameof(EntryConverter)}.", nameof(converterType));
        }

        if (converterType.GetConstructor(Type.EmptyTypes) == null)
        {
            throw new ArgumentException($"{converterType.Name} has no public parameterless constructor.", nameof(converterType));
        }

        ConverterType = converterType;
    }

    /// <summary>
    /// Type of the <see cref="EntryConverter"/> used to convert this member or all members of this type
    /// </summary>
    public Type ConverterType { get; }
}
