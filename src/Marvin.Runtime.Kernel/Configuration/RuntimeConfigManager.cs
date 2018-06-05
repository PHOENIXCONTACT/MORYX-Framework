using System;
using Marvin.Configuration;
using Marvin.Container;
using Marvin.Runtime.Configuration;

namespace Marvin.Runtime.Kernel
{
    /// <summary>
    /// Manages the configs of the runtime and its modules.
    /// </summary>
    [KernelComponent(typeof(IConfigManager), typeof(IRuntimeConfigManager))]
    public class RuntimeConfigManager : CachedConfigManager, IRuntimeConfigManager
    {
        private readonly IConfigLiveUpdate _liveUpdater = new ConfigLiveUpdater();
        private readonly SharedConfigProvider _sharedProvider;

        /// <summary>
        /// Constructor for the config manager.
        /// </summary>
        public RuntimeConfigManager()
        {
            _sharedProvider = new SharedConfigProvider(this);
            ValueProviders = new IValueProvider[]
            {
                new SharedConfigProvider(this), 
                new DefaultValueProvider(), 
            };
        }

        /// <summary>
        /// Override ValueProviders to include shared config provider
        /// </summary>
        protected override IValueProvider[] ValueProviders { get; }

        /// <summary>
        /// Fill all available emtpy properties of the config.
        /// </summary>
        /// <param name="obj">The config for which the fill process should be done.</param>
        public void FillEmpty(object obj)
        {
            ValueProviderExecutor.Execute(obj, new ValueProviderExecutorSettings().AddProviders(ValueProviders));
        }

        /// <inheritdoc />
        public IConfig GetConfiguration(Type confType, bool getCopy)
        {
            return base.GetConfiguration(confType, getCopy, confType.FullName);
        }

        /// <inheritdoc cref="CachedConfigManager.SaveConfiguration{T}(T,string)" />
        public override void SaveConfiguration<T>(T configuration, string name)
        {
            SaveConfiguration(configuration, false, name);
        }

        /// <inheritdoc />
        public void SaveConfiguration(IConfig configuration, bool liveUpdate)
        {
            SaveConfiguration(configuration, liveUpdate, configuration.GetType().FullName);
        }

        /// <summary>
        /// Save the given config and perfom if necessary a live update of the module with the config entries.
        /// </summary>
        /// <param name="configuration">The configuration which should be saved.</param>
        /// <param name="liveUpdate">Should a live update of the configuration be perfomed?</param>
        /// <param name="name">Name of the configuration</param>
        public void SaveConfiguration(IConfig configuration, bool liveUpdate, string name)
        {
            var configType = configuration.GetType();

            lock (ConfigCache)
            {
                if (liveUpdate && ConfigCache.ContainsKey(name) && typeof(IUpdatableConfig).IsAssignableFrom(configType))
                {
                    _liveUpdater.UpdateLive(configType, ConfigCache[name].Instance, configuration);
                }
                else
                {
                    ConfigCache[name] = new CacheEntry
                    {
                        Type = configType,
                        Instance = configuration,
                    };
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
                SaveConfiguration(sharedConfig, liveUpdate, sharedConfig.GetType().FullName);
            }
        }
    }
}
