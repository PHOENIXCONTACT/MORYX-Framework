// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections;
using System.Collections.Generic;
using Moryx.Logging;

namespace Moryx.Runtime.Tests.Mocks
{
    internal class TestLoggerMgmt : IModuleLogger
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
        /// Set log level of logger
        /// </summary>
        public void SetLevel(IModuleLogger logger, LogLevel level)
        {
        }

        public void SetLevel(string name, LogLevel level)
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
        }

        public LogLevel ActiveLevel { get; set; }

        public IModuleLogger Clone(Type targetType)
        {
            return this;
        }

        public void SetNotificationTarget(Action<LogLevel, string, Exception> notificationTarget)
        {
            throw new NotImplementedException();
        }
    }
}
