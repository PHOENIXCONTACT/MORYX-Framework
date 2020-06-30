// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Marvin.Configuration
{
    /// <summary>
    /// Base config manager that uses cache
    /// </summary>
    public class CachedConfigManager : ConfigManager
    {
        /// <summary>
        /// Cached config instance and type
        /// </summary>
        [DebuggerDisplay("Type = {" + nameof(Type) + "}")]
        protected class CacheEntry
        {
            /// <summary>
            /// Type of the configuration
            /// </summary>
            public Type Type { get; set; }

            /// <summary>
            /// Instance of the configuration
            /// </summary>
            public IConfig Instance { get; set; }
        }

        /// <summary>
        /// Cache of allready created config objects
        /// </summary>
        protected IDictionary<string, CacheEntry> ConfigCache { get; } = new Dictionary<string, CacheEntry>();

        /// <inheritdoc />
        protected override IConfig GetConfiguration(Type configType, bool getCopy, string name)
        {
            if (getCopy)
                return TryGetFromDirectory(configType, name);

            lock (ConfigCache)
            {
                if (ConfigCache.ContainsKey(name))
                    return ConfigCache[name].Instance;

                ConfigCache[name] = new CacheEntry
                {
                    Type = configType,
                    Instance = TryGetFromDirectory(configType, name),
                };

                return ConfigCache[name].Instance;
            }
        }

        /// <inheritdoc />
        public override void SaveConfiguration<T>(T configuration, string name)
        {
            lock (ConfigCache)
            {
                var configType = typeof(T);
                if (ConfigCache.ContainsKey(name))
                {
                    ConfigCache[name].Instance = configuration;
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

            base.SaveConfiguration(configuration, name);
        }

        /// <summary>
        /// Clear configuration cache
        /// </summary>
        public void ClearCache()
        {
            ConfigCache.Clear();
        }

        /// <summary>
        /// Save all cached config objects to disk. Config objects will only be saved to disk
        /// if the corresponding file already exists on disk.
        /// </summary>
        public void SaveAll()
        {
            try
            {
                foreach (var cacheEntry in ConfigCache.ToArray())
                {
                    var config = cacheEntry.Value.Instance as IPersistentConfig;
                    var persistConfig = config?.PersistDefaultConfig ?? true;

                    if (persistConfig || ConfigExists(cacheEntry.Key))
                    {
                        WriteToFile(cacheEntry.Value.Instance, cacheEntry.Key);
                    }
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
