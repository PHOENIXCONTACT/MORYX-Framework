using System;

namespace Marvin.Configuration
{
    /// <summary>
    /// Central component for all access to configuration objects. This should be implemented on every platform the
    /// way it works best - file, database or remote.
    /// </summary>
    public interface IConfigManager
    {
        /// <summary>
        /// Get typed configuration. Will use cached object if available
        /// </summary>
        /// <typeparam name="T">Type of config object</typeparam>
        /// <returns>Configuration object</returns>
        T GetConfiguration<T>() where T : class, IConfig, new();

        /// <summary>
        /// Get typed configuration. Also specifies behaviour for implementations with internal cache
        /// </summary>
        /// <typeparam name="T">Type of config object</typeparam>
        /// <param name="getCopy"><value>True</value>Create new instance. <value>False</value>Get from cache if possible</param>
        /// <returns>Configuration object</returns>
        T GetConfiguration<T>(bool getCopy) where T : class, IConfig, new();

        /// <summary>
        /// Save configuration
        /// </summary>
        /// <typeparam name="T">Type to save</typeparam>
        /// <param name="configuration">Object to save</param>
        void SaveConfiguration<T>(T configuration) where T : class, IConfig;
    }
}
