// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.Container;
using Moryx.Serialization;
using Moryx.Tools;
using Newtonsoft.Json;

namespace Moryx.Products.Management;

/// <summary>
/// Mapper for columns of type <see cref="string"/>
/// </summary>
[TextStrategyConfiguration]
[Component(LifeCycle.Transient, typeof(IPropertyMapper), Name = nameof(TextColumnMapper))]
internal class TextColumnMapper : ColumnMapper<string>
{
    public TextColumnMapper(Type targetType) : base(targetType)
    {
    }

    protected override IPropertyAccessor<object, string> CreatePropertyAccessor(PropertyInfo objectProp)
    {
        var propType = objectProp.PropertyType;

        // Convert Guid to string
        if (propType == typeof(Guid))
        {
            return new ConversionAccessor<string, Guid>(
                objectProp,
                guid => guid != Guid.Empty ? guid.ToString() : null,
                str => !string.IsNullOrEmpty(str) ? Guid.Parse(str) : Guid.Empty);
        }

        // Complex reference types and non-primitive structs (e.g. Vector3) -> JSON
        if (propType != typeof(string) && (propType.IsClass || propType.IsInterface || (propType.IsValueType && !propType.IsPrimitive && !propType.IsEnum)))
        {
            return new ConversionAccessor<string, object>(objectProp, SerializeToColumn, DeserializeToProperty);
        }

        // Normal string property
        return base.CreatePropertyAccessor(objectProp);
    }

    protected override object ToExpressionValue(object value)
    {
        var propType = Property.PropertyType;

        // Guid -> string
        if (propType == typeof(Guid))
        {
            return ((Guid)value).ToString();
        }

        // Complex reference types and non-primitive structs (e.g. Vector3) -> JSON
        if (propType != typeof(string) && (propType.IsClass || propType.IsInterface || (propType.IsValueType && !propType.IsPrimitive && !propType.IsEnum)))
        {
            return SerializeToColumn(value);
        }

        // Normal string / primitive conversion
        return base.ToExpressionValue(value);
    }

    /// <summary>
    /// Serialize a complex property value into the column
    /// </summary>
    /// <remarks>
    /// An empty value must stay empty in the column, otherwise the string "null"
    /// would be stored and no longer match a null comparison.
    /// </remarks>
    private string SerializeToColumn(object value)
    {
        return value != null
            ? JsonConvert.SerializeObject(value, Property.PropertyType, JsonSettings.Minimal)
            : null;
    }

    /// <summary>
    /// Deserialize the column content back into the complex property
    /// </summary>
    private object DeserializeToProperty(string column)
    {
        if (!string.IsNullOrWhiteSpace(column))
        {
            return JsonConvert.DeserializeObject(column, Property.PropertyType, JsonSettings.Minimal);
        }

        // Nullable structs can represent the empty column, all other
        // structs need their default value
        if (!Property.PropertyType.IsValueType || Nullable.GetUnderlyingType(Property.PropertyType) != null)
        {
            return null;
        }

        return Activator.CreateInstance(Property.PropertyType);
    }
}
