// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Reflection;
using Marvin.Container;
using Marvin.Serialization;
using Marvin.Tools;
using Newtonsoft.Json;

namespace Marvin.Products.Management
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
            // Convert return value to string
            if (objectProp.PropertyType.IsClass && objectProp.PropertyType != typeof(string))
                return new JsonAccessor(objectProp);

            return base.CreatePropertyAccessor(objectProp);
        }

        /// <summary>
        /// Accessor decorator to convert objects to enum and back
        /// </summary>
        private class JsonAccessor : ConversionAccessor<string, object>
        {
            public JsonAccessor(PropertyInfo property) : base(property)
            {
            }

            public override string ReadProperty(object instance)
            {
                var value = Target.ReadProperty(instance);
                return JsonConvert.SerializeObject(value, Target.Property.PropertyType, JsonSettings.Minimal);
            }

            public override void WriteProperty(object instance, string value)
            {
                var deserialized = JsonConvert.DeserializeObject(value, Target.Property.PropertyType);
                Target.WriteProperty(instance, deserialized);
            }
        }
    }
}