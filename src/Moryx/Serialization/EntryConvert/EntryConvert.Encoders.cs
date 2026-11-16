// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Concurrent;
using System.Reflection;
using Moryx.Tools;

namespace Moryx.Serialization;

/// <summary>
/// Resolution of <see cref="EntryEncoderBase"/> for EntryConvert
/// </summary>
public static partial class EntryConvert
{
    /// <summary>
    /// Encoders that are applied to all matching members without an explicit
    /// <see cref="EntrySerializeAttribute.EncoderType"/>
    /// </summary>
    private static readonly EntryEncoderBase[] _defaultEncoders =
    [
        new TypeEntryEncoder(),
        new Vector2EntryEncoder(),
        new Vector3EntryEncoder(),
        new Vector4EntryEncoder(),
        new QuaternionEntryEncoder(),
        new PlaneEntryEncoder()
    ];

    /// <summary>
    /// Struct types which are decomposed into sub-entries by the default encoders
    /// </summary>
    private static readonly HashSet<Type> _structTypes =
        _defaultEncoders.OfType<NumericStructEntryEncoder>().Select(c => c.TargetType).ToHashSet();

    /// <summary>
    /// Shared encoder instances created from <see cref="EntrySerializeAttribute.EncoderType"/>
    /// </summary>
    private static readonly ConcurrentDictionary<Type, EntryEncoderBase> _encoders = new();

    /// <summary>
    /// Resolve the <see cref="EntryEncoderBase"/> of a member. The <see cref="EntrySerializeAttribute"/> on the member
    /// takes precedence over the attribute on the member type, followed by the built-in default encoders.
    /// </summary>
    /// <param name="memberType">Either <see cref="PropertyInfo.PropertyType"/> or <see cref="ParameterInfo.ParameterType"/></param>
    /// <param name="attributeProvider">The <see cref="PropertyInfo"/> or <see cref="ParameterInfo"/> to retrieve custom attributes</param>
    /// <returns>Encoders of the member or <c>null</c>, if the member uses the default conversion</returns>
    public static EntryEncoderBase ResolveEncoder(Type memberType, ICustomAttributeProvider attributeProvider)
    {
        var encoderType = DeclaredEncoderType(attributeProvider, memberType, EntryEncoderTarget.Member)
            ?? DeclaredEncoderType(memberType, memberType, EntryEncoderTarget.Member);

        return FromEncoderType(encoderType, memberType)
            ?? _defaultEncoders.FirstOrDefault(c => c.CanEncode(memberType));
    }

    /// <summary>
    /// Resolve the <see cref="EntryEncoderBase"/> for the items of a collection member. The
    /// <see cref="EntrySerializeAttribute"/> on the member takes precedence over the attribute on the
    /// item type, followed by the built-in default encoders.
    /// </summary>
    /// <param name="memberType">Collection type of the member</param>
    /// <param name="attributeProvider">The <see cref="PropertyInfo"/> or <see cref="ParameterInfo"/> to retrieve custom attributes</param>
    /// <returns>Encoders of the items or <c>null</c>, if the items use the default conversion</returns>
    public static EntryEncoderBase ResolveItemEncoder(Type memberType, ICustomAttributeProvider attributeProvider)
    {
        if (!TryGetElementType(memberType, out var elementType))
        {
            return null;
        }

        var encoderType = DeclaredEncoderType(attributeProvider, memberType, EntryEncoderTarget.Items)
            ?? DeclaredEncoderType(elementType, elementType, EntryEncoderTarget.Member);

        return FromEncoderType(encoderType, elementType)
            ?? _defaultEncoders.FirstOrDefault(c => c.CanEncode(elementType));
    }

    private static Type DeclaredEncoderType(ICustomAttributeProvider attributeProvider, Type memberType, EntryEncoderTarget target)
    {
        var attribute = attributeProvider?.GetCustomAttribute<EntrySerializeAttribute>();
        if (attribute is null || attribute.EncoderTarget != target)
        {
            return null;
        }

        if (attribute.EncoderTarget == EntryEncoderTarget.Items)
        {
            if (attribute.EncoderType is null)
            {
                throw new InvalidOperationException(
                    $"{nameof(EntrySerializeAttribute.EncoderTarget)} {EntryEncoderTarget.Items} requires a {nameof(EntrySerializeAttribute.EncoderType)}.");
            }

            if (!TryGetElementType(memberType, out _))
            {
                throw new InvalidOperationException(
                    $"{nameof(EntrySerializeAttribute.EncoderTarget)} {EntryEncoderTarget.Items} is not supported for the non-collection type {memberType.Name}.");
            }
        }

        return attribute.EncoderType;
    }

    private static bool TryGetElementType(Type memberType, out Type elementType)
    {
        elementType = null;
        if (!IsCollection(memberType))
        {
            return false;
        }

        if (!memberType.IsArray && !memberType.IsGenericType)
        {
            return false;
        }

        elementType = ElementType(memberType);
        return elementType != null;
    }

    /// <summary>
    /// Check if the type is a struct decomposed into sub-entries by a default encoder
    /// </summary>
    private static bool IsSupportedStructType(Type type) => _structTypes.Contains(type);

    private static EntryEncoderBase FromEncoderType(Type encoderType, Type memberType)
    {
        if (encoderType == null)
        {
            return null;
        }

        var encoder = _encoders.GetOrAdd(encoderType, type => (EntryEncoderBase)Activator.CreateInstance(type));
        return encoder.CanEncode(memberType) ? encoder
            : throw new InvalidOperationException($"{encoderType.Name} can not convert members of type {memberType.Name}.");
    }
}
