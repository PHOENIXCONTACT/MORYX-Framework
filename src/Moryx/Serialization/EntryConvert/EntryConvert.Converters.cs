// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Concurrent;
using System.Reflection;
using Moryx.Tools;

namespace Moryx.Serialization;

/// <summary>
/// Resolution of <see cref="EntryConverter"/> for EntryConvert
/// </summary>
public static partial class EntryConvert
{
    /// <summary>
    /// Converters that are applied to all matching members without an explicit
    /// <see cref="EntrySerializeAttribute.ConverterType"/>
    /// </summary>
    private static readonly EntryConverter[] _defaultConverters =
    [
        new TypeEntryConverter(),
        new Vector2EntryConverter(),
        new Vector3EntryConverter(),
        new Vector4EntryConverter(),
        new QuaternionEntryConverter(),
        new PlaneEntryConverter()
    ];

    /// <summary>
    /// Struct types which are decomposed into sub-entries by the default converters
    /// </summary>
    private static readonly HashSet<Type> _structTypes =
        _defaultConverters.OfType<NumericStructEntryConverter>().Select(c => c.TargetType).ToHashSet();

    /// <summary>
    /// Shared converter instances created from <see cref="EntrySerializeAttribute.ConverterType"/>
    /// </summary>
    private static readonly ConcurrentDictionary<Type, EntryConverter> _converters = new();

    /// <summary>
    /// Resolve the <see cref="EntryConverter"/> of a member. The <see cref="EntrySerializeAttribute"/> on the member
    /// takes precedence over the attribute on the member type, followed by the built-in default converters.
    /// </summary>
    /// <param name="memberType">Either <see cref="PropertyInfo.PropertyType"/> or <see cref="ParameterInfo.ParameterType"/></param>
    /// <param name="attributeProvider">The <see cref="PropertyInfo"/> or <see cref="ParameterInfo"/> to retrieve custom attributes</param>
    /// <returns>Converter of the member or <c>null</c>, if the member uses the default conversion</returns>
    public static EntryConverter ResolveConverter(Type memberType, ICustomAttributeProvider attributeProvider)
    {
        var converterType = DeclaredConverterType(attributeProvider, memberType, EntryConverterTarget.Member)
            ?? DeclaredConverterType(memberType, memberType, EntryConverterTarget.Member);

        return FromConverterType(converterType, memberType)
            ?? _defaultConverters.FirstOrDefault(c => c.CanConvert(memberType));
    }

    /// <summary>
    /// Resolve the <see cref="EntryConverter"/> for the items of a collection member. The
    /// <see cref="EntrySerializeAttribute"/> on the member takes precedence over the attribute on the
    /// item type, followed by the built-in default converters.
    /// </summary>
    /// <param name="memberType">Collection type of the member</param>
    /// <param name="attributeProvider">The <see cref="PropertyInfo"/> or <see cref="ParameterInfo"/> to retrieve custom attributes</param>
    /// <returns>Converter of the items or <c>null</c>, if the items use the default conversion</returns>
    public static EntryConverter ResolveItemConverter(Type memberType, ICustomAttributeProvider attributeProvider)
    {
        if (!TryGetElementType(memberType, out var elementType))
        {
            return null;
        }

        var converterType = DeclaredConverterType(attributeProvider, memberType, EntryConverterTarget.Items)
            ?? DeclaredConverterType(elementType, elementType, EntryConverterTarget.Member);

        return FromConverterType(converterType, elementType)
            ?? _defaultConverters.FirstOrDefault(c => c.CanConvert(elementType));
    }

    private static Type DeclaredConverterType(ICustomAttributeProvider attributeProvider, Type memberType, EntryConverterTarget target)
    {
        var attribute = attributeProvider?.GetCustomAttribute<EntrySerializeAttribute>();
        if (attribute is null || attribute.ConverterTarget != target)
        {
            return null;
        }

        if (attribute.ConverterTarget == EntryConverterTarget.Items)
        {
            if (attribute.ConverterType is null)
            {
                throw new InvalidOperationException(
                    $"{nameof(EntrySerializeAttribute.ConverterTarget)} {EntryConverterTarget.Items} requires a {nameof(EntrySerializeAttribute.ConverterType)}.");
            }

            if (!TryGetElementType(memberType, out _))
            {
                throw new InvalidOperationException(
                    $"{nameof(EntrySerializeAttribute.ConverterTarget)} {EntryConverterTarget.Items} is not supported for the non-collection type {memberType.Name}.");
            }
        }

        return attribute.ConverterType;
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
    /// Check if the type is a struct decomposed into sub-entries by a default converter
    /// </summary>
    private static bool IsSupportedStructType(Type type) => _structTypes.Contains(type);

    private static EntryConverter FromConverterType(Type converterType, Type memberType)
    {
        if (converterType == null)
        {
            return null;
        }

        var converter = _converters.GetOrAdd(converterType, type => (EntryConverter)Activator.CreateInstance(type));
        return converter.CanConvert(memberType) ? converter
            : throw new InvalidOperationException($"{converterType.Name} can not convert members of type {memberType.Name}.");
    }
}
