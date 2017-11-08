using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Marvin.Configuration;
using Marvin.Modules;
using Marvin.Runtime.Configuration;

namespace Marvin.Runtime.Base
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
                if(propType == typeof(string))
                    continue;

                // Check for collection
                if (typeof (IEnumerable).IsAssignableFrom(propType))
                {
                    foreach (var entry in (IEnumerable) property.GetValue(instance))
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