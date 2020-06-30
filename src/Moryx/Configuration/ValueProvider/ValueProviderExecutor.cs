using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Moryx.Configuration
{
    /// <summary>
    /// Applies ValueProvider to a specific instance
    /// </summary>
    public static class ValueProviderExecutor
    {
        /// <summary>
        /// Executes configured <see cref="IValueProvider"/> and <see cref="IValueProviderFilter"/>
        /// </summary>
        /// <param name="targetObject">Instance to use</param>
        /// <param name="settings">Settings</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Execute(object targetObject, ValueProviderExecutorSettings settings)
        {
            if (settings.Providers == null)
            {
                throw new ArgumentNullException(nameof(settings.Providers));
            }

            if (settings.Filters == null)
            {
                throw new ArgumentNullException(nameof(settings.Filters));
            }

            Iterate(targetObject, settings);
        }

        private static void Iterate(object target, ValueProviderExecutorSettings settings)
        {
            foreach (var property in FilterProperties(target, settings))
            {
                foreach (var settingsProvider in settings.Providers)
                {
                    if (settingsProvider.Handle(target, property) == ValueProviderResult.Handled)
                    {
                        break;
                    }
                }

                var value = property.GetValue(target);

                if (property.PropertyType.IsValueType && !property.PropertyType.IsPrimitive ||
                     property.PropertyType.IsClass &&
                     property.PropertyType != typeof(string) &&
                     !(value is IEnumerable))
                {
                    Iterate(value, settings);
                }

                if (value is IEnumerable)
                {
                    var enumerable = value as IEnumerable;
                    foreach (var item in enumerable)
                    {
                        Iterate(item, settings);
                    }
                }
            }
        }

        private static IEnumerable<PropertyInfo> FilterProperties(object target, ValueProviderExecutorSettings settings)
        {
            var filteredProperties = new List<PropertyInfo>();

            foreach (var property in target.GetType().GetProperties(settings.PropertyBindingFlags))
            {
                if(settings.Filters.All(f => f.CheckProperty(property)))
                {
                    filteredProperties.Add(property);
                }
            }

            return filteredProperties;
        }
    }
}
