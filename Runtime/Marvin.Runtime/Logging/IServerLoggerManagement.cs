using System;
using Marvin.Logging;

namespace Marvin.Runtime
{
    /// <summary>
    /// Server specific logger management
    /// </summary>
    public interface IServerLoggerManagement : ILoggerManagement
    {
        /// <summary>
        /// Append a listener delegate to the stream of log messages
        /// </summary>
        void AppendListenerToStream(Action<ILogMessage> onMessage);

        /// <summary>
        /// Append a listener delegate to the stream of log messages with level higher or equal to given level
        /// </summary>
        void AppendListenerToStream(Action<ILogMessage> onMessage, LogLevel minLevel);

        /// <summary>
        /// Append a listener delegate to the stream of log messages from the logger with given name
        /// </summary>
        void AppendListenerToStream(Action<ILogMessage> onMessage, string name);

        /// <summary>
        /// Append a listener delegate to the stream of log messages from the logger with given name if level is equal to or higher
        /// as the given level
        /// </summary>
        void AppendListenerToStream(Action<ILogMessage> onMessage, LogLevel minLevel, string name);

        /// <summary>
        /// Remove a listener from the stream
        /// </summary>
        /// <param name="onMessage"></param>
        void RemoveListenerFromStream(Action<ILogMessage> onMessage);
    }
}