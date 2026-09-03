// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moryx.Configuration;

namespace Moryx.Runtime.Kernel.Configuration;

internal class MicrosoftExtensionsConfigProvider : IValueProvider, IContextAwareValueProvider
{
    private const string ConfigKey = "ConfigKey";
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;

    public MicrosoftExtensionsConfigProvider(
        IConfiguration configuration,
        ILogger<MicrosoftExtensionsConfigProvider> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public ValueProviderResult Handle(object config, PropertyInfo prop)
    {
        return Handle(new Stack<ExecutorLevel>([
            new(config, prop, [])
        ]));
    }

    private static object? GetDefaultValue(Type type)
    {
        if (type.IsValueType && Nullable.GetUnderlyingType(type) == null)
        {
            return Activator.CreateInstance(type); // works for structs and primitives
        }

        return null; // reference types and Nullable<T> default to null
    }

    public ValueProviderResult Handle(Stack<ExecutorLevel> levels)
    {
        var (config, prop, _) = levels.Peek();

        if (_configuration == null
            || config == null
            || !prop.CanRead
            || !prop.CanWrite)
        {
            return ValueProviderResult.Skipped;
        }

        var currentValue = prop.GetValue(config);
        var defaultValue = GetDefaultValue(prop.PropertyType);
        if (!Equals(currentValue, defaultValue)) // value is already assigned
        {
            return ValueProviderResult.Skipped;
        }

        var configKey = GetConfigKeyNameCached(levels);

        var section = _configuration.GetSection(configKey);
        if (section.Exists())
        {
            try
            {
                prop.SetValue(config, section.Get(prop.PropertyType));
                return ValueProviderResult.Handled;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to bind property {name} on {type} to configuration section {configKey}",
                    prop.Name, config.GetType().Name, configKey);
            }

        }
        else
        {
            _logger.LogDebug("Config section for {configKey} for property {name}  on {type} does not exist",
                configKey, prop.Name, config.GetType().Name);
        }

        return ValueProviderResult.Skipped;
    }

    private string GetConfigKeyNameCached(Stack<ExecutorLevel> levels)
    {
        var (_, _, dict) = levels.Peek();
        if (dict.TryGetValue(ConfigKey, out object key))
        {
            return (string)key;
        }
        var keyName = GetConfigKeyName(levels);
        dict[ConfigKey] = keyName;
        return keyName;
    }

    private string GetConfigKeyName(Stack<ExecutorLevel> levels)
    {
        var (config, prop, dict) = levels.Peek();

        bool isRooted = false;
        string key;
        if (prop is null) // can currently only occur when config is enumerable
        {
            if (dict.TryGetValue(ValueProviderExecutor.IndexKey, out var index) && index is int i)
            {
                key = i.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                throw new NotSupportedException("Szenario where prop param is not set and the dict does not contain an int is not supported.");
            }
        }
        else
        {
            key = prop.Name;

            if (prop.GetCustomAttribute<ProvidedConfigAttribute>() is ProvidedConfigAttribute attribute)
            {
                var configKey = KeyFromAttribute(config, attribute);
                if (string.IsNullOrEmpty(configKey))
                {
                    _logger.LogWarning("Config key for prop {propName} on {type} from 'ProvidedConfigAttribute' is empty. Falling back to default key",
                        prop.Name, config.GetType().Name);
                }
                else if (configKey.StartsWith(".:"))
                {
                    key = configKey.Substring(2);
                }
                else
                {
                    isRooted = true;
                    key = configKey;
                }
            }
        }

        if (isRooted)
        {
            return key;
        }
        else
        {
            var baseKey = GetRootConfigKey(levels);
            return $"{baseKey}:{key}";
        }

    }

    private string KeyFromAttribute(object config, ProvidedConfigAttribute attribute)
    {
        if (attribute.FromProperty)
        {
            var property = config.GetType().GetProperty(attribute.ConfigKey);
            if (property is null)
            {
                _logger.LogWarning("Property '{propName}' from Attribute does not exist on object of type {type}",
                    attribute.ConfigKey, config.GetType().Name);
            }
            else if (property.PropertyType != typeof(string))
            {
                _logger.LogWarning("Property '{propName}' from Attribute on object of type {type} is not of type string and can't be used as a config key",
                    attribute.ConfigKey, config.GetType().Name);
            }
            else if (!property.CanRead)
            {
                _logger.LogWarning("Property '{propName}' from Attribute on object of type {type} is not readable and can't be used as a config key",
                    attribute.ConfigKey, config.GetType().Name);
            }
            else
            {
                return property.GetValue(config) as string;
            }
            return null;
        }
        else
        {
            return attribute.ConfigKey;

        }

    }

    private string GetRootConfigKey(Stack<ExecutorLevel> levels)
    {
        if (levels.Count == 1)
        {
            var (baseObj, _, _) = levels.Peek();

            if (baseObj.GetType().GetCustomAttribute<ProvidedConfigAttribute>() is ProvidedConfigAttribute attribute)
            {
                return KeyFromAttribute(baseObj, attribute);
            }
            return baseObj.GetType().FullName;
        }
        else
        {
            var topLevel = levels.Pop();
            var configKey = GetConfigKeyNameCached(levels);
            levels.Push(topLevel);
            return configKey;
        }
    }
}

