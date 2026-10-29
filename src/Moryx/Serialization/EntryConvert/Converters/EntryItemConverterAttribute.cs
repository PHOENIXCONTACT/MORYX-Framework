// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Serialization;

/// <summary>
/// Defines the <see cref="EntryConverter"/> used to convert the elements of a collection member
/// </summary>
/// <remarks>
/// An <see cref="EntryConverterAttribute"/> on the same member converts the collection as a whole
/// and therefore takes precedence over this attribute.<br></br>
/// Only the values of a dictionary are converted, not its keys.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class EntryItemConverterAttribute : Attribute
{
    /// <summary>
    /// Create a new instance of <see cref="EntryItemConverterAttribute"/>
    /// </summary>
    /// <param name="converterType">Type deriving from <see cref="EntryConverter"/> with a public parameterless constructor</param>
    public EntryItemConverterAttribute(Type converterType)
    {
        EntryConverterAttributeValidation.ValidateConverterType(converterType);
        ConverterType = converterType;
    }

    /// <summary>
    /// Type of the <see cref="EntryConverter"/> used to convert the elements of this member
    /// </summary>
    public Type ConverterType { get; }
}
