// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.Tools;

namespace Moryx.Bindings;

/// <summary>
/// Resolve binding using optimized property access
/// </summary>
public class ReflectionResolver : BindingResolverBase
{
    /// <summary>
    /// Name of the property
    /// </summary>
    private readonly string _propertyName;

    /// <summary>
    /// Type of the last resolved source object to detect type changes
    /// </summary>
    private Type _cachedType;

    /// <summary>
    /// PropertyAccessor to access the property of the type
    /// </summary>
    private IPropertyAccessor<object, object> _cachedAccessor;

    /// <summary>
    /// Create new <see cref="ReflectionResolver"/> for a property
    /// </summary>
    public ReflectionResolver(string propertyName)
    {
        _propertyName = propertyName;
    }

    /// <inheritdoc />
    protected sealed override object Resolve(object source)
    {
        var accessor = GetAccessor(source);
        return accessor?.ReadProperty(source);
    }

    /// <inheritdoc />
    protected sealed override bool Update(object source, object value)
    {
        var accessor = GetAccessor(source);
        if (accessor == null || !accessor.Property.CanWrite)
        {
            return false;
        }

        accessor.WriteProperty(source, value);
        return true;
    }

    private IPropertyAccessor<object, object> GetAccessor(object source)
    {
        var sourceType = source.GetType();
        if (_cachedType == sourceType)
        {
            return _cachedAccessor;
        }

        var property = FindProperty(sourceType);
        _cachedAccessor = property != null ? ReflectionTool.PropertyAccessor(property) : null;
        _cachedType = sourceType;
        return _cachedAccessor;
    }

    /// <summary>
    /// Find property by name on the source type
    /// </summary>
    private PropertyInfo FindProperty(Type type)
    {
        // Find correct property by navigating down the type tree
        PropertyInfo property = null;
        const BindingFlags filter = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        while (property == null && type != null)
        {
            property = type.GetProperty(_propertyName, filter);
            type = type.BaseType;
        }

        return property;
    }
}
