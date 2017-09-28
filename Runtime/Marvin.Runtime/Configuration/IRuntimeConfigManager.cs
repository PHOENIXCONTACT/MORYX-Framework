using System;
using Marvin.Configuration;

namespace Marvin.Runtime.Configuration
{
    /// <summary>
    /// Extended config manager for the runtime
    /// </summary>
    public interface IRuntimeConfigManager : IConfigManager
    {
        /// <summary>
        /// Config directory to use for storing the serialized configurations.
        /// </summary>
        string ConfigDirectory { get; set; }

        /// <summary>
        /// Get configuration for a type computed at runtime
        /// </summary>
        /// <param name="confType">Config type</param>
        /// <param name="getCopy">Return currently active config or a temporary copy</param>
        /// <returns>Config object</returns>
        IConfig GetConfiguration(Type confType, bool getCopy);

        /// <summary>
        /// Save configuration using its type
        /// </summary>
        /// <param name="configuration">Configuration object</param>
        /// <param name="liveUpdate">Update currently active config live</param>
        void SaveConfiguration(IConfig configuration, bool liveUpdate);

        /// <summary>
        /// Save any shared configs included in this partial config
        /// </summary>
        void SaveSharedConfigs(object partialConfig, bool liveUpdate);

        /// <summary>
        /// Clear config cache for next restart
        /// </summary>
        void ClearCache();

        /// <summary>
        /// Save all open configurations
        /// </summary>
        void SaveAll();

        /// <summary>
        /// Fills all config properties with default values
        /// </summary>
        /// <param name="obj"></param>
        void FillEmptyProperties(object obj);
    }
}
