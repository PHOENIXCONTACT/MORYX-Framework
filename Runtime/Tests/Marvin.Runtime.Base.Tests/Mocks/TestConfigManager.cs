using System;
using Marvin.Configuration;
using Marvin.Runtime.Configuration;

namespace Marvin.Runtime.Base.Tests.Mocks
{
    internal class TestConfigManager : IRuntimeConfigManager
    {
        /// <summary>
        /// Config directory to use for storing the serialized configurations.
        /// </summary>
        public string ConfigDirectory { get; set; }

        /// <summary>
        /// Get typed configuration
        /// </summary>
        /// <typeparam name="T">Type of config object</typeparam>
        /// <returns>
        /// Configuration object
        /// </returns>
        public T GetConfiguration<T>() where T : class, IConfig, new()
        {
            return (T)GetConfiguration(typeof(T), true);
        }

        /// <summary>
        /// Get typed configuration. Also specifies behaviour for implementations with internal cache
        /// </summary>
        /// <typeparam name="T">Type of config object</typeparam><param name="getCopy">
        /// <value>
        /// True
        /// </value>
        /// Create new instance. 
        /// <value>
        /// False
        /// </value>
        /// Get from cache if possible</param>
        /// <returns>
        /// Configuration object
        /// </returns>
        public T GetConfiguration<T>(bool getCopy) where T : class, IConfig, new()
        {
            return GetConfiguration<T>();
        }

        /// <summary>
        /// Get configuration for a type computed at runtime
        /// </summary>
        /// <param name="confType">Config type</param>
        /// <param name="getCopy">Return currently active config or a temporary copy</param>
        /// <returns>Config object</returns>
        public IConfig GetConfiguration(Type confType, bool getCopy)
        {
            return new TestConfig
            {
                Strategy = new StrategyConfig{PluginName = "Test"},
                StrategyName = "Test"
            };
        }

        /// <summary>
        /// Save configuration
        /// </summary>
        /// <typeparam name="T">Type to save</typeparam><param name="configuration">Object to save</param>
        public void SaveConfiguration<T>(T configuration) where T : class, IConfig
        {
        }

        /// <summary>
        /// Save configuration using its type
        /// </summary>
        /// <param name="configuration">Configuration object</param>
        /// <param name="liveUpdate">Update currently active config live</param>
        public void SaveConfiguration(IConfig configuration, bool liveUpdate)
        {
        }

        /// <summary>
        /// Save any shared configs included in this partial config
        /// </summary>
        public void SaveSharedConfigs(object partialConfig, bool liveUpdate)
        {
        }

        /// <summary>
        /// Clear config cache for next restart
        /// </summary>
        public void ClearCache()
        {
        }

        /// <summary>
        /// Save all open configurations
        /// </summary>
        public void SaveAll()
        {
        }

        /// <summary>
        /// Fills all config properties with default values
        /// </summary>
        /// <param name="obj"></param>
        public void FillEmptyProperties(object obj)
        {
            
        }
    }
}
