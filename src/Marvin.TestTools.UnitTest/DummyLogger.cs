using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Marvin.Logging;

namespace Marvin.TestTools.UnitTest
{
    /// <summary>
    /// Mock for IModuleLogger implementations.
    /// </summary>
    public class DummyLogger : IModuleLogger
    {
        private readonly List<LogMessage> _messages = new List<LogMessage>();

        /// <summary>
        /// Retrieves all stored log messages.
        /// </summary>
        public IEnumerable<LogMessage> Messages
        {
            get { return _messages.ToList(); }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IModuleLogger> GetEnumerator()
        {
            return null;
        }

        /// <summary>
        /// Clears the stored log messages.
        /// </summary>
        public void ClearBuffer()
        {
            _messages.Clear();
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
        /// Creates a new logger.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IModuleLogger GetChild(string name)
        {
            return new DummyLogger();
        }

        /// <summary>
        /// Creates a new logger.
        /// </summary>
        /// <param name="childType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetChild(Type childType, string name)
        {
            return new DummyLogger();
        }

        /// <summary>
        /// Add a new entry to the log
        /// </summary>
        public void LogEntry(LogLevel level, string message, params object[] formatParameters)
        {
            Console.WriteLine($"Class: {GetCallingTypeName()}, " +
                              $"LogLevel: {level}, " +
                              $"Message: {string.Format(message, formatParameters)}");
            _messages.Add(new LogMessage(level, string.Format(message, formatParameters)));
        }

        /// <summary>
        /// Log a caught exception
        /// </summary>
        public void LogException(LogLevel level, Exception ex, string message, params object[] formatParameters)
        {
            Console.WriteLine($"Class: {GetCallingTypeName()}, " +
                              $"LogLevel: {level}, " +
                              $"Message: {string.Format(message, formatParameters)}, " +
                              $"Exception: {ex.Message}");
            _messages.Add(new LogMessage(level, string.Format(message, formatParameters), ex));
        }

        /// <summary>
        /// Will return the type name of the calling class
        /// </summary>
        private static string GetCallingTypeName()
        {
            var stackTrace = new StackTrace();
            var callingType = stackTrace.GetFrame(2).GetMethod().DeclaringType;
            return callingType != null ? callingType.Name : string.Empty;
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
        /// Active logging level
        /// </summary>
        public LogLevel ActiveLevel { get; private set; }

        /// <summary>
        /// Parent container of this child
        /// </summary>
        public IModuleLogger Parent { get; set; }

        /// <summary>
        /// Get the child with this name for a specific component
        /// </summary>
        /// <param name="name">Name of the child or empty for child with the same name</param>
        /// <param name="target">Target component the child is assigned to</param>
        public IModuleLogger GetChild(string name, Type target)
        {
            return new DummyLogger();
        }
    }

    /// <summary>
    /// Dummy implementation that hold all necessary information for a log message.
    /// </summary>
    public class LogMessage
    {
        /// <summary>
        /// The log level of this message
        /// </summary>
        public LogLevel Level { get; private set; }

        /// <summary>
        /// The formatted message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// The exception if there was one.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Creates a new LogMessage obejct.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public LogMessage(LogLevel level, string message)
        {
            Level = level;
            Message = message;
        }

        /// <summary>
        /// Creates a new LogMessage obejct.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public LogMessage(LogLevel level, string message, Exception exception)
        {
            Level = level;
            Message = message;
            Exception = exception;
        }
    }
}