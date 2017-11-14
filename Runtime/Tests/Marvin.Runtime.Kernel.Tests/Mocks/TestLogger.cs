using System;
using System.Collections;
using System.Collections.Generic;
using Marvin.Logging;

namespace Marvin.Runtime.Kernel.Tests.Mocks
{
    class TestLogger : IModuleLogger
    {
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
        /// Get the child with this name
        /// </summary>
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

        /// <summary>
        /// Add a new entry to the log
        /// </summary>
        public void LogEntry(LogLevel level, string message, params object[] formatParameters)
        {
        }

        /// <summary>
        /// Log a caught exception
        /// </summary>
        public void LogException(LogLevel level, Exception ex, string message, params object[] parameters)
        {
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
        public string Name { get; set; }

        /// <summary>
        /// Active logging level
        /// </summary>
        public LogLevel ActiveLevel { get; set; }

        /// <summary>
        /// Parent container of this child
        /// </summary>
        public IModuleLogger Parent { get; set; }
    }
}
