// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.Container;
using Moryx.Serialization;
using Moryx.Tools;
using Newtonsoft.Json;

namespace Moryx.Products.Management
{
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
            // Convert return value to string
            if (propType == typeof(Guid))
                return new ConversionAccessor<string, Guid>(objectProp,
                    guid => guid != Guid.Empty ? guid.ToString() : null,
                    str => !string.IsNullOrEmpty(str) ? Guid.Parse(str) : Guid.Empty);

            if ((propType.IsClass | propType.IsInterface) && propType != typeof(string))
                return new ConversionAccessor<string, object>(objectProp,
                    o => JsonConvert.SerializeObject(o, Property.PropertyType, JsonSettings.Minimal),
                    s => JsonConvert.DeserializeObject(s, Property.PropertyType, JsonSettings.Minimal));

            return base.CreatePropertyAccessor(objectProp);
        }

        protected override object ToExpressionValue(object value)
        {
            var propType = Property.PropertyType;
            if (propType == typeof(Guid))
                return ((Guid)value).ToString();

            if ((propType.IsClass | propType.IsInterface) && propType != typeof(string))
                return JsonConvert.SerializeObject(value, Property.PropertyType, JsonSettings.Minimal);

            return base.ToExpressionValue(value);
        }
    }
}