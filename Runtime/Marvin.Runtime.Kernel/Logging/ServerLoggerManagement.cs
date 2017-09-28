using System;
using System.Collections.Generic;
using System.Linq;
using Marvin.Configuration;
using Marvin.Container;
using Marvin.Logging;

namespace Marvin.Runtime.Kernel.Logging
{
    /// <summary>
    /// Management for the server logging. Provides mechanics to add and remove listeners for the logging. 
    /// </summary>
    [KernelComponent(typeof(ILoggerManagement), typeof(IServerLoggerManagement))]
    public class ServerLoggerManagement : LoggerManagement, IServerLoggerManagement
    {
        #region Configuration

        /// <summary>
        /// Configuration manager instance. Injected by castel.
        /// </summary>
        public IConfigManager ConfigManager { get; set; }

        private LoggingConfig _config;

        /// <summary>
        /// Get the configuration of a logger. If no configuration exist will it create a default config for the requested logger.
        /// </summary>
        /// <param name="name">The name of the logger for which the configuration should be fetched.</param>
        /// <returns>The configuration for the requested logger.</returns>
        protected override ModuleLoggerConfig GetLoggerConfig(string name)
        {
            var config = (_config = _config ?? ConfigManager.GetConfiguration<LoggingConfig>());
            var loggerConf = config.LoggerConfigs.FirstOrDefault(conf => conf.LoggerName == name);
            if (loggerConf == null)
            {
                loggerConf = new ModuleLoggerConfig { LoggerName = name, ActiveLevel = _config.DefaultLevel, ChildConfigs = new List<ModuleLoggerConfig>() };
                config.LoggerConfigs.Add(loggerConf);
            }
            return loggerConf;
        }

        /// <summary>
        /// Set the level of the given logger to the given log level.
        /// </summary>
        /// <param name="logger">The module logger for which the new log level should be set.</param>
        /// <param name="level">The new log level to which the given logger should be set.</param>
        public override void SetLevel(IModuleLogger logger, LogLevel level)
        {
            base.SetLevel(logger, level);
            ConfigManager.SaveConfiguration(_config);
        }

        #endregion

        #region Forward to listeners

        private readonly IDictionary<LogFilter, Action<ILogMessage>> _listeners =
            new Dictionary<LogFilter, Action<ILogMessage>>();

        /// <summary>
        /// Forward the incomming log message to all the filtered listeners which are interested in that log message.
        /// </summary>
        /// <param name="logMessage">The incomming log message which should be forwarded.</param>
        protected override void ForwardToListeners(ILogMessage logMessage)
        {
            IEnumerable<Action<ILogMessage>> matchingListeners;
            lock (_listeners)
            {
                matchingListeners = (from listener in _listeners
                    let filter = listener.Key
                    where MessageMatchesFilter(filter, logMessage)
                    select listener.Value).ToList();
            }

            foreach (var listener in matchingListeners)
            {
                listener(logMessage);
            }
        }

        private bool MessageMatchesFilter(LogFilter filter, ILogMessage message)
        {
            if (filter.FilterType == FilterType.None)
                return true;
            if (filter.FilterType == FilterType.NameBased)
                return message.Logger.Name == filter.Name ||
                       (message.Logger.Name.Contains(filter.Name) && message.Level >= LogLevel.Error);
            if (filter.FilterType == FilterType.LevelBased)
                return message.Level >= filter.MinLevel;
            return (message.Logger.Name.Contains(filter.Name) && message.Level >= filter.MinLevel);
        }

        /// <summary>
        /// Append a listener delegate to the stream of log messages
        /// </summary>
        public void AppendListenerToStream(Action<ILogMessage> onMessage)
        {
            var filter = new LogFilter {FilterType = FilterType.None};
            lock (_listeners)
                _listeners[filter] = onMessage;
        }

        /// <summary>
        /// Append a listener delegate to the stream of log messages with level higher or equal to given level
        /// </summary>
        public void AppendListenerToStream(Action<ILogMessage> onMessage, LogLevel minLevel)
        {
            var filter = new LogFilter {FilterType = FilterType.LevelBased, MinLevel = minLevel};
            lock (_listeners)
                _listeners[filter] = onMessage;
        }

        /// <summary>
        /// Append a listener delegate to the stream of log messages from the logger with given name
        /// </summary>
        public void AppendListenerToStream(Action<ILogMessage> onMessage, string name)
        {
            var filter = new LogFilter {FilterType = FilterType.NameBased, Name = name};
            lock (_listeners)
                _listeners[filter] = onMessage;
        }

        /// <summary>
        /// Append a listener delegate to the stream of log messages from the logger with given name if level is equal to or higher
        /// as the given level
        /// </summary>
        public void AppendListenerToStream(Action<ILogMessage> onMessage, LogLevel minLevel, string name)
        {
            var filter = new LogFilter {FilterType = FilterType.NameAndLevel, MinLevel = minLevel, Name = name};
            lock (_listeners)
                _listeners[filter] = onMessage;
        }

        /// <summary>
        /// Remove a listener from the stream. Anonymous methods can not be removed!
        /// </summary>
        /// <param name="onMessage"></param>
        public void RemoveListenerFromStream(Action<ILogMessage> onMessage)
        {
            lock (_listeners)
            {
                var matches = (from listener in _listeners
                    where listener.Value.Equals(onMessage)
                    select listener.Key).ToList();
                foreach (var match in matches)
                {
                    _listeners.Remove(match);
                }
            }
        }

        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            _listeners.Clear();
        }
    }
}
