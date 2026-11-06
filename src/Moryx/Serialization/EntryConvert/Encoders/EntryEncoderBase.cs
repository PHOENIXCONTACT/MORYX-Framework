// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Serialization;

/// <summary>
/// Base class for custom conversions of a member to and from an <see cref="Entry"/>.
/// Encoders take over the complete encoding and decoding of the member and its sub-entries.
/// </summary>
/// <remarks>
/// Encoders instances are cached and shared across all conversions.
/// Implementations must therefore be stateless and thread-safe.
/// </remarks>
public abstract class EntryEncoderBase
{
    /// <summary>
    /// Check if this encoder can encode and decode members of the given type
    /// </summary>
    /// <param name="memberType">Either <see cref="System.Reflection.PropertyInfo.PropertyType"/> or <see cref="System.Reflection.ParameterInfo.ParameterType"/></param>
    public abstract bool CanEncode(Type memberType);

    /// <summary>
    /// Encode the value of a member into an <see cref="Entry"/>.
    /// Only <see cref="Entry.Value"/>, <see cref="Entry.SubEntries"/> and <see cref="Entry.Prototypes"/> of the
    /// returned entry are used by default. Identifier and validation are always overridden from the member.
    /// Display name and description are used from the encoder if set, otherwise from the member.
    /// The read-only flag is merged, that is the entry is read-only if either the encoder or the member
    /// definition marks it as such.
    /// </summary>
    /// <param name="value">Current value of the member. Can be <c>null</c> if no instance is available.</param>
    /// <param name="context">Context of the conversion</param>
    public abstract Entry Encode(object value, EntryEncoderContext context);

    /// <summary>
    /// Decode the value of a member from an <see cref="Entry"/>
    /// </summary>
    /// <param name="entry">Encoded entry of the member</param>
    /// <param name="currentValue">Current value of the member</param>
    /// <param name="context">Context of the conversion</param>
    /// <returns>Value to assign to the member</returns>
    public abstract object Decode(Entry entry, object currentValue, EntryEncoderContext context);
}
