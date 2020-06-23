// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ComponentModel;
using System.Reflection;

namespace Moryx.Configuration
{
    /// <summary>
    /// ValueProvider that sets property's value given from a <see cref="DefaultValueAttribute"/>
    /// </summary>
    public sealed class DefaultValueProvider : IValueProvider
    {
        /// <inheritdoc />
        public ValueProviderResult Handle(object parent, PropertyInfo property)
        {
            var propType = property.PropertyType;

            // Provide default entries
            var value = property.GetValue(parent);
            if (Equals(value, DefaultValue(propType)))
            {
                value = ProvideDefaultValue(property);
                if (value == null)
                    return ValueProviderResult.Skipped;

                property.SetValue(parent, value);

                return ValueProviderResult.Handled;
            }

            return ValueProviderResult.Skipped;
        }

        private static object DefaultValue(Type propType)
        {
            return propType.IsValueType ? Activator.CreateInstance(propType) : null;
        }

        private static object ProvideDefaultValue(PropertyInfo property)
        {
            var propertyType = property.PropertyType;
            if (propertyType.IsClass && !propertyType.IsAbstract && propertyType != typeof(string))
            {
                if (propertyType.IsArray)
                {
                    var elementType = propertyType.GetElementType();

                    if (elementType == null)
                        return null;

                    return Array.CreateInstance(elementType, 0);
                }

                return Activator.CreateInstance(propertyType);
            }

            var attribute = property.GetCustomAttribute<DefaultValueAttribute>(false);
            if (attribute == null)
                return null;

            // Return if type matches
            if (property.PropertyType.IsInstanceOfType(attribute.Value))
                return attribute.Value;

            // Try to convert
            try
            {
                return Convert.ChangeType(attribute.Value, propertyType);
            }
            catch
            {
                return null;
            }
        }
    }
}
