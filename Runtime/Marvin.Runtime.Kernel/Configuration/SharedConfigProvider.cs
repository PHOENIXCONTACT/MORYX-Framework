using System.Collections.Generic;
using System.Reflection;
using Marvin.Configuration;
using Marvin.Runtime.Configuration;

namespace Marvin.Runtime.Kernel
{
    internal class SharedConfigProvider
    {
        private readonly IRuntimeConfigManager _configManager;
        public SharedConfigProvider(IRuntimeConfigManager configManager)
        {
            _configManager = configManager;
        }

        internal bool CheckForSharedConfig(object parent, PropertyInfo property)
        {
            var sharedAtt = property.GetCustomAttribute<SharedConfigAttribute>();
            if (sharedAtt == null || !typeof(IConfig).IsAssignableFrom(property.PropertyType))
                return false;

            var sharedConf = _configManager.GetConfiguration(property.PropertyType, sharedAtt.UseCopy);
            property.SetValue(parent, sharedConf);
            return true;
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