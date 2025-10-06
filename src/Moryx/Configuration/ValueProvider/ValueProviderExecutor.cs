// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using System.Reflection;

namespace Moryx.Configuration
{
    /// <summary>
    /// Applies ValueProvider to a specific instance
    /// </summary>
    public class ValueProviderExecutor : IEmptyPropertyProvider
    {
        private readonly ValueProviderExecutorSettings _settings;

        /// <summary>
        /// Create provider instance with <paramref name="settings"/>
        /// </summary>
        /// <param name="settings">Settings for the provider</param>
        public ValueProviderExecutor(ValueProviderExecutorSettings settings)
        {
            _settings = settings;
        }

        /// <inheritdoc />
        public void FillEmpty(object obj)
        {
            Execute(obj, _settings);
        }

        /// <summary>
        /// Executes configured <see cref="IValueProvider"/> and <see cref="IValueProviderFilter"/>
        /// </summary>
        /// <param name="targetObject">Instance to use</param>
        /// <param name="settings">Settings</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Execute(object targetObject, ValueProviderExecutorSettings settings)
        {
            if (targetObject is null)
            {
                throw new ArgumentNullException(nameof(targetObject));
            }

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
                    try
                    {
                        if (settingsProvider.Handle(target, property) == ValueProviderResult.Handled)
                        {
                            break;
                        }
                    }
                    catch (Exception)
                    {
                        // TODO: Restrict exceception type
                        // TODO: Consider enabling logging
                    }
                }

                var value = property.GetValue(target);
                // Iterate each item of an enumerable
                if (value is IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                    {
                        if (item != null)
                            Iterate(item, settings);
                    }
                }
                else if (value != null && property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    Iterate(value, settings);
                }
            }
        }

        private static IEnumerable<PropertyInfo> FilterProperties(object target, ValueProviderExecutorSettings settings)
        {
            var filteredProperties = new List<PropertyInfo>();
            foreach (var property in target.GetType().GetProperties(settings.PropertyBindingFlags))
            {
                if (settings.Filters.All(f => f.CheckProperty(property)))
                {
                    filteredProperties.Add(property);
                }
            }

            return filteredProperties;
        }
    }
}

