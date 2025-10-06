// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Reflection;

namespace Moryx.Configuration
{
    /// <summary>
    /// Applies values from <see cref="DefaultValueAttribute"/> on the properties
    /// </summary>
    public sealed class DefaultValueAttributeProvider : IValueProvider
    {
        /// <inheritdoc/>
        public ValueProviderResult Handle(object parent, PropertyInfo property)
        {
            var propertyType = property.PropertyType;
            var value = property.GetValue(parent);

            var defaultValue = propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null;
            // We only operate on properties currently having their system default
            if (!Equals(value, defaultValue))
                return ValueProviderResult.Skipped;

            // No attribute, nothing we can do
            var attribute = property.GetCustomAttribute<DefaultValueAttribute>(false);
            if (attribute == null)
                return ValueProviderResult.Skipped;

            // Accept nulls for objects or nullable reference types
            if (attribute.Value == null && (propertyType.IsClass || Nullable.GetUnderlyingType(propertyType) != null))
            {
                // If the default is the system default, we just keep it
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
                var converted = Convert.ChangeType(attribute.Value, propertyType);
                property.SetValue(parent, converted);
                return ValueProviderResult.Handled;
            }
            catch
            {
                return ValueProviderResult.Skipped;
            }

        }
    }
}
