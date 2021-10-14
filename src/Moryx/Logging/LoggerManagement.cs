// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moryx.Tools;

namespace Moryx.Logging
{
    internal interface ILogTargetFactory
    {
        ILogTarget Create(string name);
    }

    /// <summary>
    /// Framework component managing the life cycle of all loggers and provide diagnostic access to them
    /// </summary>
    public abstract class LoggerManagement : ILoggerManagement, ILogTargetFactory, IDisposable
    {
        private readonly Thread _loggingThread;
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        /// <summary>
        /// Constructor for the LoggerManagement.
        /// </summary>
        protected LoggerManagement()
        {
            _loggingThread = new Thread(ProcessQueue)
            {
                IsBackground = true,
                Name = "LoggingProcessor",
            };
            _loggingThread.Start();
        }

        #region LogTargetFactory

        ILogTarget ILogTargetFactory.Create(string name)
        {
            return CreateLogTarget(name);
        }

        /// <summary>
        /// Creates a log target for the internal loggers
        /// </summary>
        protected abstract ILogTarget CreateLogTarget(string name);

        #endregion

        #region Logger access

        private Logger GetOrCreateLogger(string name, Type targetType)
        {
            lock (_loggers)
            {
                Logger logger;
                if (_loggers.ContainsKey(name))
                    logger = _loggers[name];
                else
                {
                    var loggerConf = GetLoggerConfig(name);
                    logger = new Logger(loggerConf, targetType, this, _logQueue);
                    _loggers[name] = logger;
                }
                return logger;
            }
        }

        /// <summary>
        /// Determine the config for this logger
        /// </summary>
        protected abstract ModuleLoggerConfig GetLoggerConfig(string name);

        /// <summary>
        /// Set log level of logger by its name
        /// </summary>
        public void SetLevel(string name, LogLevel level)
        {
            var loggerTree = name.Split('.');
            IModuleLogger logger = _loggers[loggerTree[0]];
            foreach (var partialName in loggerTree.Skip(1))
                logger = logger.First(l => l.Name.EndsWith(partialName));

            SetLevel(logger, level);
        }

        /// <summary>
        /// Set log level of logger
        /// </summary>
        public virtual void SetLevel(IModuleLogger logger, LogLevel level)
        {
            SetLevelRecursive(logger, level);
        }

        private void SetLevelRecursive(IModuleLogger logger, LogLevel level)
        {
            var casted = logger as Logger;
            if (casted == null)
                return;

            casted.ActiveLevel = level;
            // The childrens level must be at least the parents level
            foreach (var child in logger.Where(child => child.ActiveLevel > level))
            {
                SetLevelRecursive(child, level);
            }
        }

        /// <summary>
        /// Activate logging for a module
        /// </summary>
        public void ActivateLogging(ILoggingHost module)
        {
            var logger = GetOrCreateLogger(module.Name, module.GetType());
            module.Logger = logger;
        }

        /// <summary>
        /// Deactivate logging for a plugin
        /// </summary>
        public void DeactivateLogging(ILoggingHost module)
        {
            var logger = module.Logger as Logger;
            logger?.ClearChildren();

            module.Logger = new IdleLogger(module.Name, CreateLogTarget(module.Name));
        }

        #endregion

        #region Stream listener

        private readonly BlockingCollection<LogMessage> _logQueue = new BlockingCollection<LogMessage>();

        private void ProcessQueue()
        {
            var token = _tokenSource.Token;
            while (!token.IsCancellationRequested)
            {
                LogMessage logMessage;
                try
                {
                    logMessage = _logQueue.Take(_tokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                logMessage.Format();
                try
                {
                    ForwardToLogTarget(logMessage);
                    ForwardToListeners(logMessage);
                }
                catch (Exception ex)
                {
                    // Well, how do we log if the logger throws exceptions. :D
                    CrashHandler.WriteErrorToFile("Caught exception while logging! Exception: " + ex.Message);
                }
            }
        }

        private static void ForwardToLogTarget(LogMessage logMessage)
        {
            // Forward message to internal log and all appender
            var logTarget = logMessage.LogTarget;
            if (logMessage.IsException)
            {
                logTarget.Log(logMessage.Level, logMessage.LoggerMessage, logMessage.Exception);
            }
            else
            {
                logTarget.Log(logMessage.Level, logMessage.LoggerMessage);
            }
        }

        /// <summary>
        /// Forword log messages to the listeners.
        /// </summary>
        /// <param name="logMessage">The log message which should be forworded.</param>
        protected abstract void ForwardToListeners(ILogMessage logMessage);

        #endregion

        #region IEnumerable

        private readonly IDictionary<string, Logger> _loggers = new Dictionary<string, Logger>();

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IModuleLogger> GetEnumerator()
        {
            lock (_loggers)
            {
                var copy = _loggers.Values.ToList();
                return copy.GetEnumerator();
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            _tokenSource.Cancel();
            _loggingThread.Join(2000);
        }
    }
}
