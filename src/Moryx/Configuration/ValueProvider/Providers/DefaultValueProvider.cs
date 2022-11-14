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
                if(ProvideDefaultValue(property, out value))
                {
                    property.SetValue(parent, value);

                    return ValueProviderResult.Handled;
                }
            }

            return ValueProviderResult.Skipped;
        }

        private static object DefaultValue(Type propType)
        {
            return propType.IsValueType ? Activator.CreateInstance(propType) : null;
        }

        private static bool ProvideDefaultValue(PropertyInfo property, out object defaultValue)
        {
            var propertyType = property.PropertyType;
            defaultValue = null;

            var attribute = property.GetCustomAttribute<DefaultValueAttribute>(false);
            if(attribute != null)
            {
                // Accept nulls for objects or nullable reference types
                if(attribute.Value == null && (propertyType.IsClass || Nullable.GetUnderlyingType(propertyType) != null)) {
                    return true;
                }

                // Return if type matches
                if (property.PropertyType.IsInstanceOfType(attribute.Value))
                {
                    defaultValue = attribute.Value;
                    return true;
                }

                

                // Try to convert
                try
                {
                    defaultValue = Convert.ChangeType(attribute.Value, propertyType);
                    return true;
                }
                catch
                {
                    return false;
                }
            }


            if (propertyType.IsClass && !propertyType.IsAbstract && propertyType != typeof(string))
            {
                if (propertyType.IsArray)
                {
                    var elementType = propertyType.GetElementType();

                    if (elementType == null)
                    {
                        return false;
                    }
                        

                    defaultValue = Array.CreateInstance(elementType, 0);
                    return true;
                }

                defaultValue = Activator.CreateInstance(propertyType);
                return true;

            }

            return false;
        }
    }
}
