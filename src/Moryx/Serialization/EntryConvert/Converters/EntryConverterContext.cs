// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;

namespace Moryx.Serialization;

/// <summary>
/// Context of a conversion executed by an <see cref="EntryConverter"/>
/// </summary>
public sealed class EntryConverterContext(Type memberType, ICustomAttributeProvider attributeProvider,
    ICustomSerialization serialization, object instance)
{
    /// <summary>
    /// Either <see cref="PropertyInfo.PropertyType"/> or <see cref="ParameterInfo.ParameterType"/> of the converted
    /// member
    /// </summary>
    public Type MemberType { get; } = memberType;

    /// <summary>
    /// The <see cref="PropertyInfo"/> or <see cref="ParameterInfo"/> of the converted member to retrieve custom
    /// attributes
    /// </summary>
    public ICustomAttributeProvider AttributeProvider { get; } = attributeProvider;

    /// <summary>
    /// Serialization used for the current conversion
    /// </summary>
    public ICustomSerialization Serialization { get; } = serialization;

    /// <summary>
    /// Instance declaring the member. Can be <c>null</c> if no instance is available.
    /// </summary>
    public object Instance { get; } = instance;
}
