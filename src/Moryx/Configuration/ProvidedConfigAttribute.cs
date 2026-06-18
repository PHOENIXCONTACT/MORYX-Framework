// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Configuration;

/// <summary>
/// Attribute that controls how the marked Property or Class should be filled by
/// Values contained in the IConfiguration of the Host
/// </summary>
/// <param name="configKey"><see cref="ConfigKey"/></param>
/// <param name="fromProperty"><see cref="FromProperty"/></param>
/// <example>
/// Use abolute config key
/// <code>
/// [ProvidedConfig("Secrets:ApiKey")]
/// </code>
/// Use path relative to the current object
/// <code>
/// [ProvidedConfig(".:ApiKey")]
/// </code>
/// Load the actual config key from the `ConfigKey` property
/// <code>
/// [ProvidedConfig(nameof(ConfigKey), fromProperty: true)]
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
public class ProvidedConfigAttribute(string configKey, bool fromProperty = false) : Attribute
{
    /// <summary>
    /// Config key to load. Either contains the config key directly or if <see cref="FromProperty"/> is set it contains the name of a property that contains the config key
    /// </summary>
    public string ConfigKey { get; } = configKey;

    /// <summary>
    /// Determines the interpretation of ConfigKey
    /// </summary>
    public bool FromProperty { get; } = fromProperty;
}
