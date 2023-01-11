// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Runtime.Configuration;
using Moryx.Serialization;
using Newtonsoft.Json;

namespace Moryx.Runtime.Kernel
{
    /// <summary>
    /// Manages the configs of the runtime and its modules.
    /// </summary>
    [KernelComponent(typeof(IConfigManager))]
    public class ConfigManager : IConfigManager, IEmptyPropertyProvider
    {
        private readonly ConfigLiveUpdater _liveUpdater = new ConfigLiveUpdater();
        private readonly SharedConfigProvider _sharedProvider;

        /// <summary>
        /// Extension of the config files used in the MORYX
        /// </summary>
        public const string FileExtension = ".json";

        /// <summary>
        /// Constructor for the config manager.
        /// </summary>
        public ConfigManager()
        {
            _sharedProvider = new SharedConfigProvider(this);
        }

        /// <summary>
        /// Override ValueProviders to include shared config provider
        /// </summary>
        protected IValueProvider[] ValueProviders => new IValueProvider[]
        {
            _sharedProvider,
            new DefaultValueAttributeProvider(),
            new ActivatorValueProvider()
        };

        /// <summary>
        /// Directory used to read and write config files
        /// </summary>
        public string ConfigDirectory { get; set; }

        /// <summary>
        /// Cache of allready created config objects
        /// </summary>
        protected IDictionary<string, IConfig> ConfigCache { get; } = new Dictionary<string, IConfig>();

        /// <summary>
        /// Fill all available empty properties of the config.
        /// </summary>
        /// <param name="obj">The config for which the fill process should be done.</param>
        public void FillEmpty(object obj)
        {
            ValueProviderExecutor.Execute(obj, new ValueProviderExecutorSettings().AddProviders(ValueProviders));
        }

        /// <summary>
        /// Generic get configuration method
        /// </summary>
        public IConfig GetConfiguration(Type configType, string name, bool getCopy)
        {
            if (getCopy)
                return TryGetFromDirectory(configType, name);

            lock (ConfigCache)
            {
                if (ConfigCache.ContainsKey(name))
                    return ConfigCache[name];

                ConfigCache[name] = TryGetFromDirectory(configType, name);

                return ConfigCache[name];
            }
        }

        /// <summary>
        /// Save the given config and perform if necessary a live update of the module with the config entries.
        /// </summary>
        /// <param name="configuration">The configuration which should be saved.</param>
        /// <param name="liveUpdate">Should a live update of the configuration be performed?</param>
        /// <param name="name">Name of the configuration</param>
        public void SaveConfiguration(IConfig configuration, string name, bool liveUpdate)
        {
            var configType = configuration.GetType();

            lock (ConfigCache)
            {
                if (liveUpdate && ConfigCache.ContainsKey(name) && typeof(IUpdatableConfig).IsAssignableFrom(configType))
                {
                    _liveUpdater.UpdateLive(configType, ConfigCache[name], configuration);
                }
                else
                {
                    ConfigCache[name] = configuration;
                }
            }

            SaveSharedConfigs(configuration, liveUpdate);
            WriteToFile(configuration, name);
        }

        /// <summary>
        /// Save any shared configs included in this partial config
        /// </summary>
        public void SaveSharedConfigs(object partialConfig, bool liveUpdate)
        {
            foreach (var sharedConfig in _sharedProvider.IncludedSharedConfigs(partialConfig))
            {
                SaveConfiguration(sharedConfig, sharedConfig.GetType().FullName, liveUpdate);
            }
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
                    config = (IConfig)JsonConvert.DeserializeObject(fileContent, confType, JsonSettings.ReadableReplace);

                    ValueProviderExecutor.Execute(config, new ValueProviderExecutorSettings().AddProviders(ValueProviders));
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
            ValueProviderExecutor.Execute(config, new ValueProviderExecutorSettings().AddProviders(ValueProviders));

            return config;
        }

        /// <summary>
        /// Write config object to json file
        /// </summary>
        protected void WriteToFile(object config, string name)
        {
            var text = JsonConvert.SerializeObject(config, JsonSettings.Readable);
            File.WriteAllText(GetConfigPath(name), text);
        }

        private string GetConfigPath(string name)
        {
            var configName = $"{name}{FileExtension}";
            return Path.Combine(ConfigDirectory, configName);
        }

        /// <summary>
        /// Checks whether the config exists
        /// </summary>
        /// <param name="name">Name of the configuration</param>
        /// <returns></returns>
        protected bool ConfigExists(string name)
        {
            return File.Exists(GetConfigPath(name));
        }
    }
}
