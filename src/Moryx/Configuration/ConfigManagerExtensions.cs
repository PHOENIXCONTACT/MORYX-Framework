// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Configuration
{
    /// <summary>
    /// Extension overloads that restore APIs previously defined in `IConfigManager`
    /// </summary>
    public static class ConfigManagerExtensions
    {
        /// <summary>
        /// Get typed configuration. Will use cached object if available
        /// </summary>
        /// <typeparam name="T">Type of config object</typeparam>
        /// <returns>Configuration object</returns>
        public static T GetConfiguration<T>(this IConfigManager configManager)
            where T : ConfigBase, new()
        {
            var configType = typeof(T);
            return (T)configManager.GetConfiguration(configType, configType.FullName, false);
        }

        /// <summary>
        /// Get typed configuration. Will use cached object if available
        /// </summary>
        /// <typeparam name="T">Type of config object</typeparam>
        /// <param name="configManager">Config manager instance</param>
        /// <param name="name">Name of the config and target file</param>
        /// <returns>Configuration object</returns>
        public static T GetConfiguration<T>(this IConfigManager configManager, string name)
            where T : ConfigBase, new()
        {
            var configType = typeof(T);
            return (T)configManager.GetConfiguration(configType, name, false);
        }

        /// <summary>
        /// Get typed configuration. Also specifies behaviour for implementations with internal cache
        /// </summary>
        /// <typeparam name="T">Type of config object</typeparam>
        /// <param name="configManager">Config manager instance</param>
        /// <param name="getCopy"><value>True</value>Create new instance. <value>False</value>Get from cache if possible</param>
        /// <returns>Configuration object</returns>
        public static T GetConfiguration<T>(this IConfigManager configManager, bool getCopy)
            where T : ConfigBase, new()
        {
            var configType = typeof(T);
            return (T)configManager.GetConfiguration(configType, configType.FullName, getCopy);
        }

        /// <summary>
        /// Get typed configuration. Also specifies behaviour for implementations with internal cache
        /// </summary>
        /// <typeparam name="T">Type of config object</typeparam>
        /// <param name="configManager">Config manager instance</param>
        /// <param name="getCopy"><value>True</value>Create new instance. <value>False</value>Get from cache if possible</param>
        /// <param name="name">Will lookup the config by the given name</param>
        /// <returns>Configuration object</returns>
        public static T GetConfiguration<T>(this IConfigManager configManager, bool getCopy, string name)
            where T : ConfigBase, new()
        {
            return (T)configManager.GetConfiguration(typeof(T), name, getCopy);
        }

        /// <summary>
        /// Get typed configuration. Also specifies behaviour for implementations with internal cache
        /// </summary>
        /// <param name="configManager">Config manager instance</param>
        /// <param name="configType">Type of config object</param>
        /// <param name="getCopy"><value>True</value>Create new instance. <value>False</value>Get from cache if possible</param>
        /// <returns>Configuration object</returns>
        public static ConfigBase GetConfiguration(this IConfigManager configManager, Type configType, bool getCopy)
        {
            return configManager.GetConfiguration(configType, configType.FullName, getCopy);
        }

        /// <summary>
        /// Save configuration
        /// </summary>
        /// <param name="configManager">Config manager instance</param>
        /// <param name="configuration">Object to save</param>
        public static void SaveConfiguration(this IConfigManager configManager, ConfigBase configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            configManager.SaveConfiguration(configuration, configuration.GetType().FullName, false);
        }

        /// <summary>
        /// Save configuration
        /// </summary>
        /// <param name="configManager">Config manager instance</param>
        /// <param name="configuration">Object to save</param>
        /// <param name="liveUpdate">Flag if config should be updated on the currently used object reference</param>
        public static void SaveConfiguration(this IConfigManager configManager, ConfigBase configuration, bool liveUpdate)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            configManager.SaveConfiguration(configuration, configuration.GetType().FullName, liveUpdate);
        }

        /// <summary>
        /// Save configuration
        /// </summary>
        /// <param name="configManager">Config manager instance</param>
        /// <param name="configuration">Object to save</param>
        /// <param name="name">Will save the configuration under the given name</param>
        public static void SaveConfiguration(this IConfigManager configManager, ConfigBase configuration, string name)
        {
            configManager.SaveConfiguration(configuration, name, false);
        }
    }
}
