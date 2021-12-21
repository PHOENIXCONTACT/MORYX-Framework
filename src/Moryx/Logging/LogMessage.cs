// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Tools;

namespace Moryx.Logging
{
    internal class LogMessage : ILogMessage
    {
        private readonly string _message;
        private readonly object[] _parameters;

        #region Internal helper properties

        public ILogTarget LogTarget { get; set; }

        public bool IsException => Exception != null;

        public Exception Exception { get; }

        #endregion

        #region ILogMessage

        /// <inheritdoc />
        public IModuleLogger Logger { get; }

        /// <inheritdoc />
        public string ClassName { get; }

        /// <inheritdoc />
        public LogLevel Level { get; }

        /// <inheritdoc />
        public string Message { get; private set; }

        /// <inheritdoc />
        public DateTime Timestamp { get; }

        #endregion

        public LogMessage(IModuleLogger logger, string className, LogLevel level, Exception exception, string message, object[] formatParameters)
            : this(logger, className, level, message, formatParameters)
        {
            Exception = exception;
        }

        public LogMessage(IModuleLogger logger, string className, LogLevel level, string message, object[] parameters)
        {
            _message = message;
            _parameters = parameters;

            // Set properties
            Logger = logger;
            Level = level;
            ClassName = className;
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// Perform all internal formatting operations
        /// </summary>
        internal void Format()
        {
            // Format message
            try
            {
                Message = string.Format(_message, _parameters);
            }
            catch
            {
                // Someone failed to write a working format string
                Message = _message + " - Format failed!";
            }
        }
    }
}
