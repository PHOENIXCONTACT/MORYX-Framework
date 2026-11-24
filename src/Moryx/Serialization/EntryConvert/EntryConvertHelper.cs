// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Reflection;
using Moryx.Tools;

namespace Moryx.Serialization;

/// <summary>
/// Stateless reflection and conversion helpers used by <see cref="EntryConvert"/>
/// </summary>
internal static class EntryConvertHelper
{
    /// <summary>
    /// Check if a property is read-only, either because it has no setter or by <see cref="ReadOnlyAttribute"/>
    /// </summary>
    public static bool IsReadOnly(PropertyInfo property)
    {
        if (!property.CanWrite)
        {
            return true;
        }

        var readOnlyAtt = property.GetCustomAttribute<ReadOnlyAttribute>();
        return readOnlyAtt?.IsReadOnly ?? false;
    }

    /// <summary>
    /// Try to read the value of a property. Faulty getters return the exception as value.
    /// </summary>
    public static bool TryGetValue(PropertyInfo property, object instance, out object value)
    {
        try
        {
            value = instance != null ? property.GetValue(instance) : null;
            return true;
        }
        catch (Exception ex)
        {
            value = ex;
            return false;
        }
    }

    /// <summary>
    /// Determine the element type of arrays and generic collections
    /// </summary>
    public static bool TryGetElementType(Type memberType, out Type elementType)
    {
        elementType = null;
        if (!EntryConvert.IsCollection(memberType))
        {
            return false;
        }

        if (!memberType.IsArray && !memberType.IsGenericType)
        {
            return false;
        }

        elementType = EntryConvert.ElementType(memberType);
        return elementType != null;
    }

    /// <summary>
    /// Get the name of a collection item from its display name, its own ToString() or its type name
    /// </summary>
    public static string GetItemName(object item)
    {
        var itemType = item.GetType();

        // Value type do not have names
        if (EntryConvert.ValueOrStringType(itemType))
        {
            return itemType.Name;
        }

        // Check for display name declaration
        var displayName = itemType.GetDisplayName();
        if (!string.IsNullOrWhiteSpace(displayName))
        {
            return displayName;
        }

        // Check if item declares its own version of ToString()
        return itemType.GetMethod(nameof(ToString))!.DeclaringType == typeof(object) ? itemType.Name : item.ToString();
    }

    /// <summary>
    /// Read a stream as Base64
    /// </summary>
    public static string ConvertToBase64(Stream stream)
    {
        var bytes = new byte[stream.Length];
        stream.ReadExactly(bytes, 0, (int)stream.Length);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Default value of a parameter or <c>null</c> if it has none
    /// </summary>
    public static object DefaultArgument(ParameterInfo parameter) =>
        parameter.HasDefaultValue ? parameter.DefaultValue : null;

    /// <summary>
    /// Checks if the return type of the method is a task
    /// </summary>
    public static bool IsAsyncMethod(MethodInfo method) =>
        typeof(Task).IsAssignableFrom(method.ReturnType);
}
