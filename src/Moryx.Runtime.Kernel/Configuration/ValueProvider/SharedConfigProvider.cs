// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.Configuration;
using Moryx.Runtime.Configuration;

namespace Moryx.Runtime.Kernel
{
    internal class SharedConfigProvider : IValueProvider
    {
        private readonly IConfigManager _configManager;

        public SharedConfigProvider(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        public ValueProviderResult Handle(object parent, PropertyInfo property)
        {
            var sharedAtt = property.GetCustomAttribute<SharedConfigAttribute>();
            if (sharedAtt == null || !typeof(IConfig).IsAssignableFrom(property.PropertyType))
                return ValueProviderResult.Skipped;

            var sharedConf = _configManager.GetConfiguration(property.PropertyType, sharedAtt.UseCopy);
            property.SetValue(parent, sharedConf);
            return ValueProviderResult.Handled;
        }

        internal IEnumerable<IConfig> IncludedSharedConfigs(object parent)
        {
            var sharedConfigs = new List<IConfig>();
            ScanType(parent, sharedConfigs);
            return sharedConfigs;
        }

        private void ScanType(object instance, List<IConfig> foundShared)
        {
            foreach (var property in instance.GetType().GetProperties())
            {
                var sharedAtt = property.GetCustomAttribute<SharedConfigAttribute>();
                if (sharedAtt == null || sharedAtt.UseCopy)
                    continue;

                var config = (IConfig)property.GetValue(instance);
                if (!foundShared.Contains(config))
                    foundShared.Add(config);
            }
        }
    }
}
