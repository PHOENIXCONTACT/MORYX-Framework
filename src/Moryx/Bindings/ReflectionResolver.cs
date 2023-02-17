// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Reflection;

namespace Moryx.Bindings
{
    /// <summary>
    /// Resolve binding using reflection
    /// </summary>
    public class ReflectionResolver : BindingResolverBase
    {
        /// <summary>
        /// Name of the property
        /// </summary>
        private readonly string _propertyName;

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
            var property = GetProperty(source);
            return property == null ? null : property.GetValue(source);
        }

        /// <inheritdoc />
        protected sealed override bool Update(object source, object value)
        {
            var property = GetProperty(source);
            if (property == null || !property.CanWrite)
                return false;

            if (value is IConvertible && typeof(IConvertible).IsAssignableFrom(property.PropertyType))
                property.SetValue(source, Convert.ChangeType(value, property.PropertyType));
            else
                property.SetValue(source, value);
            return true;
        }

        /// <summary>
        /// Find property by name on the source object
        /// </summary>
        private PropertyInfo GetProperty(object source)
        {
            var type = source.GetType();

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
}
