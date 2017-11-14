using System;
using System.Collections;
using System.Collections.Generic;
using Marvin.Logging;

namespace Marvin.Runtime.Kernel.Tests.Mocks
{
    internal class TestLoggerMgmt : ILoggerManagement
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
        /// Get the top level logger with this name. If this was the first request a logger will be created
        /// </summary>
        public IModuleLogger GetLogger(string name)
        {
            return GetLogger(name, LogLevel.Info);
        }

        /// <summary>
        /// Get the top level logger with this name. If this was the first request a logger will be created
        /// </summary>
        public IModuleLogger GetLogger(string name, LogLevel level)
        {
            return new TestLogger {Name = name, ActiveLevel = level};
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
            module.Logger = GetLogger(module.Name);
        }

        /// <summary>
        /// Deactivate logging for a Module
        /// </summary>
        /// <param name="module"/>
        public void DeactivateLogging(ILoggingHost module)
        {
        }

        /// <summary>
        /// Append a listener delegate to the stream of log messages
        /// </summary>
        public void AppendListenerToStream(Action<ILogMessage> onMessage)
        {
        }

        /// <summary>
        /// Append a listener delegate to the stream of log messages with level higher or equal to given level
        /// </summary>
        public void AppendListenerToStream(Action<ILogMessage> onMessage, LogLevel minLevel)
        {
        }

        /// <summary>
        /// Append a listener delegate to the stream of log messages from the logger with given name
        /// </summary>
        public void AppendListenerToStream(Action<ILogMessage> onMessage, string name)
        {
        }

        /// <summary>
        /// Append a listener delegate to the stream of log messages from the logger with given name if level is equal to or higher
        ///             as the given level
        /// </summary>
        public void AppendListenerToStream(Action<ILogMessage> onMessage, LogLevel minLevel, string name)
        {
        }

        /// <summary>
        /// Remove a listener from the stream
        /// </summary>
        /// <param name="onMessage"/>
        public void RemoveListenerFromStream(Action<ILogMessage> onMessage)
        {
        }
    }
}
