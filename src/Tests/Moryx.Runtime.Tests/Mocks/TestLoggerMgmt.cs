// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections;
using System.Collections.Generic;
using Marvin.Logging;

namespace Marvin.Runtime.Tests.Mocks
{
    internal class TestLoggerMgmt : IServerLoggerManagement, IModuleLogger
    {
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IModuleLogger> GetEnumerator()
        {
            return new List<IModuleLogger>().GetEnumerator();
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

        /// <summary>
        /// Set log level of logger
        /// </summary>
        public void SetLevel(IModuleLogger logger, LogLevel level)
        {
        }

        public void SetLevel(string name, LogLevel level)
        {
        }

        /// <summary>
        /// Activate logging for a Module
        /// </summary>
        /// <param name="module"/>
        public void ActivateLogging(ILoggingHost module)
        {
            Name = module.Name;
            ActiveLevel = LogLevel.Info;
            module.Logger = this;
        }

        /// <summary>
        /// Deactivate logging for a Module
        /// </summary>
        /// <param name="module"/>
        public void DeactivateLogging(ILoggingHost module)
        {
        }

        private Action<ILogMessage> _listener;
        /// <summary>
        /// Append a listener delegate to the stream of log messages
        /// </summary>
        public void AppendListenerToStream(Action<ILogMessage> onMessage)
        {
            _listener = onMessage;
        }

        /// <summary>
        /// Append a listener delegate to the stream of log messages with level higher or equal to given level
        /// </summary>
        public void AppendListenerToStream(Action<ILogMessage> onMessage, LogLevel minLevel)
        {
            _listener = onMessage;
        }

        /// <summary>
        /// Append a listener delegate to the stream of log messages from the logger with given name
        /// </summary>
        public void AppendListenerToStream(Action<ILogMessage> onMessage, string name)
        {
            _listener = onMessage;
        }

        /// <summary>
        /// Append a listener delegate to the stream of log messages from the logger with given name if level is equal to or higher
        ///             as the given level
        /// </summary>
        public void AppendListenerToStream(Action<ILogMessage> onMessage, LogLevel minLevel, string name)
        {
            _listener = onMessage;
        }

        /// <summary>
        /// Remove a listener from the stream
        /// </summary>
        /// <param name="onMessage"/>
        public void RemoveListenerFromStream(Action<ILogMessage> onMessage)
        {
        }

        public IModuleLogger GetChild(string name, Type target)
        {
            return this;
        }

        public IModuleLogger Parent { get; set; }
 
        public string Name { get; set; }

        public void Log(LogLevel level, string message, params object[] formatParameters)
        {
            LogException(level, null, message, formatParameters);
        }

        public void LogException(LogLevel level, Exception ex, string message, params object[] formatParameters)
        {
            var msg = new LogMessage
            {
                Level = level,
                ClassName = "Blub",
                Exception = ex,
                Logger = this,
                Message = string.Format(message, formatParameters)
            };

            if (level >= LogLevel.Warning)
                _listener(msg);
        }

        public LogLevel ActiveLevel { get; set; }

        public IModuleLogger Clone(Type targetType)
        {
            return this;
        }

        private class LogMessage : ILogMessage
        {
            public IModuleLogger Logger { get; set; }
            public string ClassName { get; set; }
            public LogLevel Level { get; set; }
            public string Message { get; set; }
            public Exception Exception { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}
