using System;
using System.ComponentModel;
using System.Reflection;

namespace Marvin.Configuration
{
    /// <summary>
    /// Provides default values for empty properties found in configuration classs
    /// </summary>
    public class DefaultValueProvider
    {
        /// <summary>
        /// Check if this property can be filled with a default value
        /// </summary>
        public static bool CheckPropertyForDefault(object parentObject, PropertyInfo property)
        {
            var propType = property.PropertyType;

            // Provide default entries
            var value = property.GetValue(parentObject);
            if (object.Equals(value, DefaultValue(propType)))
            {
                value = ProvideDefaultValue(property);
                if (value == null || !property.CanWrite)
                    return true;

                property.SetValue(parentObject, value);
            }

            return !propType.IsClass || propType == typeof(string);
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
