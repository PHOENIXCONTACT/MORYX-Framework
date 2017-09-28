using Marvin.Configuration;
using Marvin.Container;
using Marvin.Runtime.Configuration;

namespace Marvin.Runtime.Kernel.Configuration
{
    /// <summary>
    /// Manages the configs of the runtime and its modules.
    /// </summary>
    [KernelComponent(typeof(IConfigManager), typeof(IRuntimeConfigManager))]
    public class RuntimeConfigManager : CachedConfigManager, IRuntimeConfigManager
    {
        private readonly IConfigLiveUpdate _liveUpdater = new ConfigLiveUpdater();

        private readonly NodeProcessor[] _processors;
        private readonly SharedConfigProvider _sharedProvider;

        /// <summary>
        /// Constructor for the config manager.
        /// </summary>
        public RuntimeConfigManager()
        {
            _sharedProvider = new SharedConfigProvider(this);
            _processors = new NodeProcessor[] { _sharedProvider.CheckForSharedConfig, DefaultValueProvider.CheckPropertyForDefault };
        }

        /// <summary>
        /// Override NodeProcessors to include shared config provider
        /// </summary>
        protected override NodeProcessor[] NodeProcessors { get { return _processors; } }

        /// <summary>
        /// Fill all available emtpy properties of the config.
        /// </summary>
        /// <param name="obj">The config for which the fill process should be done.</param>
        public void FillEmptyProperties(object obj)
        {
            ValueProvider.FillProperties(obj, NodeProcessors);
        }

        /// <summary>
        /// Save the given configuration of type T.
        /// </summary>
        /// <typeparam name="T">Type of the configuration.</typeparam>
        /// <param name="configuration">The configuration of type T.</param>
        public override void SaveConfiguration<T>(T configuration)
        {
            SaveConfiguration(configuration, false);
        }

        /// <summary>
        /// Save the given config and perfom if necessary a live update of the module with the config entries.
        /// </summary>
        /// <param name="configuration">The configuration which should be saved.</param>
        /// <param name="liveUpdate">Should a live update of the configuration be perfomed?</param>
        public void SaveConfiguration(IConfig configuration, bool liveUpdate)
        {
            var type = configuration.GetType();
            if (liveUpdate && ConfigCache.ContainsKey(type) && typeof(IUpdatableConfig).IsAssignableFrom(type))
                _liveUpdater.UpdateLive(type, ConfigCache[type], configuration);
            else
                ConfigCache[type] = configuration;

            SaveSharedConfigs(configuration, liveUpdate);
            WriteToFile(type, configuration);
        }

        /// <summary>
        /// Save any shared configs included in this partial config
        /// </summary>
        public void SaveSharedConfigs(object partialConfig, bool liveUpdate)
        {
            foreach (var sharedConfig in _sharedProvider.IncludedSharedConfigs(partialConfig))
            {
                SaveConfiguration(sharedConfig, liveUpdate);
            }
        }
    }
}
