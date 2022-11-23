// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ComponentModel;
using System.Reflection;

namespace Moryx.Configuration
{
    /// <summary>
    /// Applies values from <see cref="DefaultValueAttribute"/> on the properties
    /// </summary>
    public sealed class DefaultValueAttributeProvider : IValueProvider
    {
        private static object DefaultValue(Type propType)
        {
            return propType.IsValueType ? Activator.CreateInstance(propType) : null;
        }

        /// <inheritdoc/>
        public ValueProviderResult Handle(object parent, PropertyInfo property)
        {
            var propertyType = property.PropertyType;
            var value = property.GetValue(parent);
            
            if (Equals(value, DefaultValue(propertyType)))
            {
                var attribute = property.GetCustomAttribute<DefaultValueAttribute>(false);
                if (attribute != null)
                {
                    // Accept nulls for objects or nullable reference types
                    if (attribute.Value == null && (propertyType.IsClass || Nullable.GetUnderlyingType(propertyType) != null))
                    {
                        property.SetValue(parent, value);
                        return ValueProviderResult.Handled;
                    }

                    // Return if type matches
                    if (property.PropertyType.IsInstanceOfType(attribute.Value))
                    {
                        property.SetValue(parent, attribute.Value);
                        return ValueProviderResult.Handled;
                    }

                    // Try to convert
                    try
                    {
                        property.SetValue(parent, Convert.ChangeType(attribute.Value, propertyType));
                        return ValueProviderResult.Handled;
                    }
                    catch
                    {
                    }
                }
            }
            return ValueProviderResult.Skipped;
        }
    }
}
