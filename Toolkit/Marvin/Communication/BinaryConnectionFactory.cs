using System;
using Marvin.Communication.Serial;
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
                case nameof(SerialBinaryConnection):
                    instance = new SerialBinaryConnection(validator) { Logger = logger.GetChild(string.Empty, typeof(SerialBinaryConnection)) };
                    break;
                default:
                    throw new ArgumentException("Unknown plugin name in config!", "config");
            }

            instance.Initialize(config);
            return instance;
        }
    }
}