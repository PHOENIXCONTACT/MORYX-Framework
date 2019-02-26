using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Marvin.Configuration;
using Marvin.Container;
using Marvin.Logging;

namespace Marvin.Runtime.Kernel
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
        /// Creates the log target where log messages will be sent to
        /// </summary>
        protected override ILogTarget CreateLogTarget(string name)
        {
            return new LogTarget(name);
        }

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

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private readonly IDictionary<LogFilter, Action<ILogMessage>> _listeners =
            new Dictionary<LogFilter, Action<ILogMessage>>();

        /// <summary>
        /// Forward the incomming log message to all the filtered listeners which are interested in that log message.
        /// </summary>
        /// <param name="logMessage">The incomming log message which should be forwarded.</param>
        protected override void ForwardToListeners(ILogMessage logMessage)
        {
            _lock.EnterReadLock();
            var matchingListeners = (from listener in _listeners
                                     where listener.Key.Match(logMessage)
                                     select listener.Value).ToList();
            _lock.ExitReadLock();

            foreach (var listener in matchingListeners)
            {
                listener(logMessage);
            }
        }

        /// <summary>
        /// Append a listener delegate to the stream of log messages
        /// </summary>
        public void AppendListenerToStream(Action<ILogMessage> onMessage)
        {
            var filter = new LogFilter.Full();
            _lock.EnterWriteLock();
            _listeners[filter] = onMessage;
            _lock.ExitWriteLock();
        }

        /// <summary>
        /// Append a listener delegate to the stream of log messages with level higher or equal to given level
        /// </summary>
        public void AppendListenerToStream(Action<ILogMessage> onMessage, LogLevel minLevel)
        {
            var filter = new LogFilter.Level(minLevel);
            _lock.EnterWriteLock();
            _listeners[filter] = onMessage;
            _lock.ExitWriteLock();
        }

        /// <summary>
        /// Append a listener delegate to the stream of log messages from the logger with given name
        /// </summary>
        public void AppendListenerToStream(Action<ILogMessage> onMessage, string name)
        {
            var filter = new LogFilter.Name(name);
            _lock.EnterWriteLock();
            _listeners[filter] = onMessage;
            _lock.ExitWriteLock();
        }

        /// <summary>
        /// Append a listener delegate to the stream of log messages from the logger with given name if level is equal to or higher
        /// as the given level
        /// </summary>
        public void AppendListenerToStream(Action<ILogMessage> onMessage, LogLevel minLevel, string name)
        {
            var filter = new LogFilter.NameAndLevel(minLevel, name);
            _lock.EnterWriteLock();
            _listeners[filter] = onMessage;
            _lock.ExitWriteLock();
        }

        /// <summary>
        /// Remove a listener from the stream. Anonymous methods can not be removed!
        /// </summary>
        /// <param name="onMessage"></param>
        public void RemoveListenerFromStream(Action<ILogMessage> onMessage)
        {
            _lock.EnterWriteLock();
            var matches = (from listener in _listeners
                           where listener.Value.Equals(onMessage)
                           select listener.Key).ToList();
            foreach (var match in matches)
            {
                _listeners.Remove(match);
            }
            _lock.ExitWriteLock();
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
