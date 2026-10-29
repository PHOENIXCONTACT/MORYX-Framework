// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Serialization;

/// <summary>
/// Defines the <see cref="EntryConverter"/> used to convert a member or all members of a type.
/// </summary>
/// <remarks>
/// A converter defined on a member takes precedence over a converter defined on the member's type.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
public sealed class EntryConverterAttribute : Attribute
{
    /// <summary>
    /// Create a new instance of <see cref="EntryConverterAttribute"/>
    /// </summary>
    /// <param name="converterType">Type deriving from <see cref="EntryConverter"/> with a public parameterless constructor</param>
    public EntryConverterAttribute(Type converterType)
    {
        EntryConverterAttributeValidation.ValidateConverterType(converterType);
        ConverterType = converterType;
    }

    /// <summary>
    /// Type of the <see cref="EntryConverter"/> used to convert this member or all members of this type
    /// </summary>
    public Type ConverterType { get; }
}
