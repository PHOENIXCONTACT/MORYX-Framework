// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Reflection;

namespace Moryx.Configuration
{
    /// <summary>
    /// Uses the activator on properties of classes with a default constructor
    /// </summary>
    public sealed class ActivatorValueProvider : IValueProvider
    {
        /// <inheritdoc/>
        public ValueProviderResult Handle(object parent, PropertyInfo property)
        {

            var propertyType = property.PropertyType;
            var value = property.GetValue(parent);

            if (propertyType.IsClass && value == null && !propertyType.IsAbstract && propertyType != typeof(string))
            {
                if (propertyType.IsArray)
                {
                    var elementType = propertyType.GetElementType();

                    if (elementType == null)
                    {
                        return ValueProviderResult.Skipped;
                    }


                    property.SetValue(parent, Array.CreateInstance(elementType, 0));
                    return ValueProviderResult.Handled;
                }

                var ctor = propertyType.GetConstructor(Type.EmptyTypes);
                if (ctor != null)
                {
                    property.SetValue(parent, Activator.CreateInstance(propertyType));
                    return ValueProviderResult.Handled;
                }
            }
            return ValueProviderResult.Skipped;
        }
    }
}
