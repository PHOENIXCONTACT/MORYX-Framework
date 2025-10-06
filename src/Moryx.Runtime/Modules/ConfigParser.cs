// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using System.Reflection;
using Moryx.Configuration;
using Moryx.Modules;
using Moryx.Runtime.Configuration;

namespace Moryx.Runtime.Modules
{
    internal class ConfigParser
    {
        internal static void ParseStrategies(IConfig moduleConfig, IDictionary<Type, string> containerConfig)
        {
            ParseEntry(moduleConfig, containerConfig);
        }

        private static void ParseEntry(object instance, IDictionary<Type, string> containerConfig)
        {
            foreach (var property in instance.GetType().GetProperties())
            {
                var propType = property.PropertyType;

                // Check for class properties
                if (!propType.IsClass)
                    continue;

                // Check for plugin config and strategy attribute
                ModuleStrategyAttribute attribute;
                if ((attribute = property.GetCustomAttribute<ModuleStrategyAttribute>()) != null)
                {
                    var pluginName = typeof(IPluginConfig).IsAssignableFrom(propType)
                        ? ((IPluginConfig)property.GetValue(instance)).PluginName
                        : (string)property.GetValue(instance);
                    containerConfig[attribute.Strategy] = pluginName;
                }

                // Now filter strings
                if (propType == typeof(string))
                    continue;

                // Check for collection
                if (typeof(IEnumerable).IsAssignableFrom(propType))
                {
                    foreach (var entry in (IEnumerable)property.GetValue(instance))
                    {
                        ParseEntry(entry, containerConfig);
                    }
                }
                else // Simple property
                {
                    ParseEntry(property.GetValue(instance), containerConfig);
                }
            }
        }
    }
}
