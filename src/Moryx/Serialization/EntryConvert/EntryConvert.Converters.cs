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
    /// Converters that are applied to all matching members without an explicit <see cref="EntryConverterAttribute"/>
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
    /// Shared converter instances created from <see cref="EntryConverterAttribute"/>
    /// </summary>
    private static readonly ConcurrentDictionary<Type, EntryConverter> _converters = new();

    /// <summary>
    /// Resolve the <see cref="EntryConverter"/> of a member. The <see cref="EntryConverterAttribute"/> on the member
    /// takes precedence over the attribute on the member type, followed by the built-in default converters.
    /// </summary>
    /// <param name="memberType">Either <see cref="PropertyInfo.PropertyType"/> or <see cref="ParameterInfo.ParameterType"/></param>
    /// <param name="attributeProvider">The <see cref="PropertyInfo"/> or <see cref="ParameterInfo"/> to retrieve custom attributes</param>
    /// <returns>Converter of the member or <c>null</c>, if the member uses the default conversion</returns>
    public static EntryConverter ResolveConverter(Type memberType, ICustomAttributeProvider attributeProvider)
    {
        var converter = FromAttribute(attributeProvider, memberType) ?? FromAttribute(memberType, memberType);
        return converter ?? _defaultConverters.FirstOrDefault(c => c.CanConvert(memberType));
    }

    /// <summary>
    /// Check if the type is a struct decomposed into sub-entries by a default converter
    /// </summary>
    private static bool IsSupportedStructType(Type type) => _structTypes.Contains(type);

    private static EntryConverter FromAttribute(ICustomAttributeProvider attributeProvider, Type memberType)
    {
        var attribute = attributeProvider?.GetCustomAttribute<EntryConverterAttribute>();
        if (attribute == null)
        {
            return null;
        }

        var converter = _converters.GetOrAdd(attribute.ConverterType, type => (EntryConverter)Activator.CreateInstance(type));
        return converter.CanConvert(memberType) ? converter
            : throw new InvalidOperationException($"{attribute.ConverterType.Name} can not convert members of type {memberType.Name}.");
    }
}
