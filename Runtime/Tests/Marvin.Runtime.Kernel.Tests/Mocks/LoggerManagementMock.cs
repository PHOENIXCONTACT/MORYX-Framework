using System;
using System.Collections;
using System.Collections.Generic;
using Marvin.Logging;

namespace Marvin.Runtime.Kernel.Tests.Mocks
{
    public class LoggerManagementMock : ILoggerManagement
    {
        public IEnumerator<IModuleLogger> GetEnumerator()
        {
            return new List<IModuleLogger>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void ActivateLogging(ILoggingHost module)
        {
            module.Logger = new LoggerMock();
        }

        public void DeactivateLogging(ILoggingHost module)
        {
        }

        public void SetLevel(IModuleLogger logger, LogLevel level)
        {
        }

        public void SetLevel(string name, LogLevel level)
        {
        }
    }

    public class LoggerMock : IModuleLogger
    {
        public IEnumerator<IModuleLogger> GetEnumerator()
        {
            return new List<IModuleLogger>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IModuleLogger GetChild(string name, Type target)
        {
            return this;
        }

        public IModuleLogger Parent { get; set; }

        public void LogEntry(LogLevel level, string message, params object[] formatParameters)
        {
        }

        public void LogException(LogLevel level, Exception ex, string message, params object[] formatParameters)
        {
        }

        /// <summary>
        /// Clone this instance to be used on a new object
        /// </summary>
        public IModuleLogger Clone(Type targetType)
        {
            return this;
        }

        public string Name { get; private set; }

        public LogLevel ActiveLevel { get; private set; }
    }
}