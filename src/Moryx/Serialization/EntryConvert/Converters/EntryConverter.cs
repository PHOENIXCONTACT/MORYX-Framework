// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Serialization;

/// <summary>
/// Base class for custom conversions of a member to and from an <see cref="Entry"/>.
/// Converters take over the complete encoding and decoding of the member and its sub-entries.
/// </summary>
/// <remarks>
/// Converter instances are cached and shared across all conversions.
/// Implementations must therefore be stateless and thread-safe.
/// </remarks>
public abstract class EntryConverter
{
    /// <summary>
    /// Check if this converter can convert members of the given type
    /// </summary>
    /// <param name="memberType">Either <see cref="System.Reflection.PropertyInfo.PropertyType"/> or <see cref="System.Reflection.ParameterInfo.ParameterType"/></param>
    public abstract bool CanConvert(Type memberType);

    /// <summary>
    /// Encode the value of a member into an <see cref="Entry"/>.
    /// Only <see cref="Entry.Value"/>, <see cref="Entry.SubEntries"/> and <see cref="Entry.Prototypes"/> of the
    /// returned entry are used. Identifier, display name, description, validation and the read-only flag
    /// are overriden afterwards from the member.
    /// </summary>
    /// <param name="value">Current value of the member. Can be <c>null</c> if no instance is available.</param>
    /// <param name="context">Context of the conversion</param>
    public abstract Entry Encode(object value, EntryConverterContext context);

    /// <summary>
    /// Decode the value of a member from an <see cref="Entry"/>
    /// </summary>
    /// <param name="entry">Encoded entry of the member</param>
    /// <param name="currentValue">Current value of the member</param>
    /// <param name="context">Context of the conversion</param>
    /// <returns>Value to assign to the member</returns>
    public abstract object Decode(Entry entry, object currentValue, EntryConverterContext context);
}
