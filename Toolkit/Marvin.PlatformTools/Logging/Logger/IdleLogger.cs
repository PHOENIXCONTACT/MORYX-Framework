using System;
using System.Collections;
using System.Collections.Generic;
using Common.Logging;

namespace Marvin.Logging
{
    /// <summary>
    /// This is used to replace the logger implementation on a plugin while it idles
    /// </summary>
    internal class IdleLogger : IModuleLogger
    {
        private ILog _internalLog;

        public IdleLogger(string name)
        {
            Name = name;
            _internalLog = LogFactory.Create(name);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IModuleLogger> GetEnumerator()
        {
            yield break;
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
        /// Clone this instance to be used on a new object
        /// </summary>
        public IModuleLogger Clone(Type targetType)
        {
            return this;
        }

        /// <summary>
        /// Name of this logger instance
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Add a new entry to the log
        /// </summary>
        public void LogEntry(LogLevel level, string message, params object[] formatParameters)
        {
            var logMessage = new LogMessage(this, "IdleLogger", level, message, formatParameters);
            LogMessage(logMessage);
        }

        /// <summary>
        /// Log a caught exception
        /// </summary>
        public void LogException(LogLevel level, Exception ex, string message, params object[] parameters)
        {
            var logMessage = new LogMessage(this, "IdleLogger", level, ex, message, parameters);
            LogMessage(logMessage);
        }

        private void LogMessage(LogMessage logMessage)
        {
            if (logMessage.IsException)
                _internalLog.Error(logMessage.LoggerMessage, logMessage.Exception);
            else
                _internalLog.Error(logMessage.LoggerMessage);
        }

        /// <summary>
        /// Active logging level
        /// </summary>
        public LogLevel ActiveLevel { get { return LogLevel.Fatal; } }

        /// <summary>
        /// Create a new child logger or return an exisiting one with this name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IModuleLogger GetChild(string name)
        {
            return this;
        }

        /// <summary>
        /// Get the child with this name
        /// </summary>
        public IModuleLogger GetChild(string name, Type targetType)
        {
            return this;
        }

        public IModuleLogger Parent { get; set; }
    }
}
