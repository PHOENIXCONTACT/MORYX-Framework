using System;
using Marvin.Logging;

namespace Marvin.Communication
{
    /// <summary>
    /// Static alternative to the dependency injection factory.
    /// </summary>
    public static class BinaryConnectionFactory
    {
        /// <summary>
        /// Create a binary connection for given config and additional dependencies
        /// </summary>
        public static IBinaryConnection Create(BinaryConnectionConfig config, IMessageValidator validator, IModuleLogger logger)
        {
            IBinaryConnection instance;
            switch (config.PluginName)
            {
                default:
                    throw new ArgumentException("Unknown plugin name in config!", "config");
            }

            instance.Initialize(config);
            return instance;
        }
    }
}