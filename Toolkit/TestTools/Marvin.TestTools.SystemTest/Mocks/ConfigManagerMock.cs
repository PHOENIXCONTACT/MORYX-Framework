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
        /// Create new config manager mock
        /// </summary>
        public ConfigManagerMock()
        {
            AvailableConfigs = new List<IConfig>();
        }

        /// <summary>
        /// Gets or sets the configuration to return.
        /// </summary>
        /// <value>
        /// The configuration to return.
        /// </value>
        public List<IConfig> AvailableConfigs { get; private set; }  

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetConfiguration<T>() where T : class, IConfig, new()
        {
            return GetConfiguration<T>(true);
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="getCopy">if set to <c>true</c> [get copy].</param>
        /// <returns></returns>
        public T GetConfiguration<T>(bool getCopy) where T : class, IConfig, new()
        {
            return (T)GetConfiguration(typeof(T), getCopy);
        }

        /// <summary>
        /// Saves the configuration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration">The configuration.</param>
        public void SaveConfiguration<T>(T configuration) where T : class, IConfig
        {
        }

        /// <summary>
        /// Get configuration for a type computed at runtime
        /// </summary>
        /// <param name="confType">Config type</param>
        /// <param name="getCopy">Return currently active config or a temporary copy</param>
        /// <returns>
        /// Config object
        /// </returns>
        public IConfig GetConfiguration(Type confType, bool getCopy)
        {
            return AvailableConfigs.FirstOrDefault(c => c.GetType() == confType);
        }

        /// <summary>
        /// Save configuration using its type
        /// </summary>
        /// <param name="type">Config type</param>
        /// <param name="configuration">Configuration object</param>
        /// <param name="liveUpdate">Update currently active config live</param>
        public void SaveConfiguration(Type type, IConfig configuration, bool liveUpdate)
        {
        }

        /// <summary>
        /// Fill all empty properties of the config object using the attribute
        /// </summary>
        /// <param name="obj">Target object</param>
        public void FillEmptyProperties(object obj)
        {
        }
    }
}
