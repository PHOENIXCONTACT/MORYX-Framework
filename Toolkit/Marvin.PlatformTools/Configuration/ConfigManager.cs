using System;
using System.IO;
using Marvin.Serialization;

namespace Marvin.Configuration
{
    /// <summary>
    /// Basic config manager to be used by platforms to provide typed configuration
    /// </summary>
    public class ConfigManager : IConfigManager
    {
        /// <summary>
        /// Directory used to read and write config files
        /// </summary>
        public string ConfigDirectory { get; set; }

        /// <summary>
        /// Array of processors used to scan the config for missing nodes
        /// </summary>
        protected virtual NodeProcessor[] NodeProcessors { get; } = { DefaultValueProvider.CheckPropertyForDefault };

        /// <summary>
        /// Get the config of type T.
        /// </summary>
        /// <typeparam name="T">Type T of the configuration which is wanted.</typeparam>
        /// <returns>The config of type T.</returns>
        public T GetConfiguration<T>() where T : class, IConfig, new()
        {
            return GetConfiguration<T>(false);
        }

        /// <summary>
        /// Get the config of type T.
        /// </summary>
        /// <typeparam name="T">Type T of the configuration which is wanted.</typeparam>
        /// <param name="getCopy">Flag to get a configuration copy.</param>
        /// <returns>The config of type T.</returns>
        public T GetConfiguration<T>(bool getCopy) where T : class, IConfig, new()
        {
            return GetConfiguration(typeof(T), getCopy) as T;
        }

        /// <summary>
        /// Generic get configuration method
        /// </summary>
        /// <param name="configType">The type of the configuration.</param>
        /// <param name="getCopy">Flag to get a configuration copy.</param>
        /// <returns>The config of type T.</returns>
        public virtual IConfig GetConfiguration(Type configType, bool getCopy)
        {
            return TryGetFromDirectory(configType);
        }

        /// <summary>
        /// Try to read config from directory or create default replacement
        /// </summary>
        protected virtual IConfig TryGetFromDirectory(Type confType)
        {
            // Get or create config object
            IConfig config;
            var configPath = GetConfigPath(confType);
            if (File.Exists(configPath))
            {
                try
                {
                    var fileContent = File.ReadAllText(configPath);
                    config = (IConfig)Json.Deserialize(fileContent, confType, JsonSettings.ReadableReplace);
                    ValueProvider.FillProperties(config, NodeProcessors);
                }
                catch (Exception e)
                {
                    config = CreateConfig(confType, ConfigState.Error, e.Message);
                }
            }
            else
            {
                config = CreateConfig(confType, ConfigState.Generated, "Config file not found! Running on default values.");
            }

            return config;
        }

        private IConfig CreateConfig(Type confType, ConfigState state, string loadError)
        {
            var config = (IConfig)Activator.CreateInstance(confType);
            config.ConfigState = state;
            config.LoadError = loadError;

            // Initialize ConfigBase
            var configBase = config as ConfigBase;
            configBase?.Initialize();

            // Fill default values
            ValueProvider.FillProperties(config, NodeProcessors);
            
            return config;
        }

        /// <summary>
        /// Saves a configuration of type T.
        /// </summary>
        /// <typeparam name="T">The type of the configuration.</typeparam>
        /// <param name="configuration">The configuration of type T.</param>
        public virtual void SaveConfiguration<T>(T configuration) where T : class, IConfig
        {
            WriteToFile(typeof(T), configuration);
        }

        /// <summary>
        /// Write config object to mcf file
        /// </summary>
        protected void WriteToFile(Type confType, object config)
        {
            var text = Json.Serialize(config, JsonSettings.Readable);
            File.WriteAllText(GetConfigPath(confType), text);
        }

        private string GetConfigPath(Type type)
        {
            var configName = $"{type.FullName}{ConfigConstants.FileExtension}";
            return Path.Combine(ConfigDirectory, configName);
        }
    }
}
