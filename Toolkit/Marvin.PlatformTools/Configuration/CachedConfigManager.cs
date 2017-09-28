using System;
using System.Collections.Generic;
using System.Linq;

namespace Marvin.Configuration
{
    /// <summary>
    /// Base config manager that uses cache
    /// </summary>
    public class CachedConfigManager : ConfigManager
    {
        /// <summary>
        /// Cache of allready created config objects
        /// </summary>
        protected IDictionary<Type, IConfig> ConfigCache { get; } = new Dictionary<Type, IConfig>();

        /// <summary>
        /// Gets a configuration of the wanted type.
        /// </summary>
        /// <param name="confType">The type of the configuration.</param>
        /// <param name="getCopy">Copy or not copy, that is here the question.</param>
        /// <returns>The wanted configuration.</returns>
        public override IConfig GetConfiguration(Type confType, bool getCopy)
        {
            if (getCopy)
                return TryGetFromDirectory(confType);

            lock (ConfigCache)
            {
                var config = ConfigCache.ContainsKey(confType) ? ConfigCache[confType] : null;
                if (config != null)
                    return config;

                config = TryGetFromDirectory(confType);
                ConfigCache[confType] = config;

                return config;
            }
        }

        /// <summary>
        /// Save the configuration of type T from the parameter list.
        /// </summary>
        /// <typeparam name="T">The type of the configuration.</typeparam>
        /// <param name="configuration">The configuration of tyxpe T which should be saved.</param>
        public override void SaveConfiguration<T>(T configuration)
        {
            ConfigCache[typeof (T)] = configuration;
            base.SaveConfiguration(configuration);
        }

        /// <summary>
        /// Clear configuration cache
        /// </summary>
        public void ClearCache()
        {
            ConfigCache.Clear();
        }

        /// <summary>
        /// Save all cached config objects to disk
        /// </summary>
        public void SaveAll()
        {
            try
            {
                foreach (var config in ConfigCache.Values.ToArray())
                {
                    WriteToFile(config.GetType(), config);
                }
            }
            // During shutdown we just want to avoid a crash message
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
            }
        }
    }
}