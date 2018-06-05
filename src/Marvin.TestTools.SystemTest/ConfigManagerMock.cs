using System;
using System.Collections.Generic;
using System.Linq;
using Marvin.Configuration;

namespace Marvin.TestTools.SystemTest.Mocks
{
    /// <summary>
    /// Mock of a config manger to use with the unit of work factory
    /// </summary>
    public class ConfigManagerMock : IConfigManager
    {
        /// <summary>
        /// Gets or sets the configuration to return.
        /// </summary>
        public Dictionary<string, IConfig> AvailableConfigs { get; } = new Dictionary<string, IConfig>();

        public T GetConfiguration<T>() where T : class, IConfig, new()
        {
            return GetConfiguration<T>(true, typeof(T).FullName);
        }

        public T GetConfiguration<T>(string name) where T : class, IConfig, new()
        {
            return GetConfiguration<T>(true, name);
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="getCopy">if set to <c>true</c> [get copy].</param>
        /// <returns></returns>
        public T GetConfiguration<T>(bool getCopy) where T : class, IConfig, new()
        {
            return GetConfiguration<T>(getCopy, typeof(T).FullName);
        }

        public T GetConfiguration<T>(bool getCopy, string name) where T : class, IConfig, new()
        {
            return (T)GetConfiguration(name);
        }

        /// <summary>
        /// Saves the configuration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration">The configuration.</param>
        public void SaveConfiguration<T>(T configuration) where T : class, IConfig
        {
            SaveConfiguration(configuration, typeof(T).FullName);
        }

        public void SaveConfiguration<T>(T configuration, string name) where T : class, IConfig
        {
            AvailableConfigs[name] = configuration;
        }

        /// <summary>
        /// Get configuration for a type computed at runtime
        /// </summary>
        private IConfig GetConfiguration(string name)
        {
            return AvailableConfigs.ContainsKey(name) ? AvailableConfigs[name] : null;
        }
    }
}