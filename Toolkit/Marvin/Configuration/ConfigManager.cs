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
        protected virtual NodeProcessor[] NodeProcessors { get; } =
        {
            DefaultValueProvider.CheckPropertyForDefault
        };

        /// <inheritdoc />
        public T GetConfiguration<T>() where T : class, IConfig, new()
        {
            return GetConfiguration<T>(typeof(T).FullName);
        }

        /// <inheritdoc />
        public T GetConfiguration<T>(string name) where T : class, IConfig, new()
        {
            return GetConfiguration<T>(false, name);
        }

        /// <inheritdoc />
        public T GetConfiguration<T>(bool getCopy) where T : class, IConfig, new()
        {
            return GetConfiguration<T>(getCopy, typeof(T).FullName);
        }

        /// <inheritdoc />
        public T GetConfiguration<T>(bool getCopy, string name) where T : class, IConfig, new()
        {
            return GetConfiguration(typeof(T), getCopy, name) as T;
        }

        /// <summary>
        /// Generic get configuration method
        /// </summary>
        protected virtual IConfig GetConfiguration(Type configType, bool getCopy, string name)
        {
            return TryGetFromDirectory(configType, name);
        }

        /// <inheritdoc />
        public void SaveConfiguration<T>(T configuration) where T : class, IConfig
        {
            SaveConfiguration(configuration, typeof(T).FullName);
        }

        /// <inheritdoc />
        public virtual void SaveConfiguration<T>(T configuration, string name) where T : class, IConfig
        {
            WriteToFile(configuration, name);
        }

        /// <summary>
        /// Try to read config from directory or create default replacement
        /// </summary>
        protected virtual IConfig TryGetFromDirectory(Type confType, string name)
        {
            // Get or create config object
            IConfig config;
            var configPath = GetConfigPath(name);

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
        /// Write config object to mcf file
        /// </summary>
        protected void WriteToFile(object config, string name)
        {
            var text = Json.Serialize(config, JsonSettings.Readable);
            File.WriteAllText(GetConfigPath(name), text);
        }

        private string GetConfigPath(string name)
        {
            var configName = $"{name}{ConfigConstants.FileExtension}";
            return Path.Combine(ConfigDirectory, configName);
        }
    }
}