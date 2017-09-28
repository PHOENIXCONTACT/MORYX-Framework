using System;
using Common.Logging;

namespace Marvin.Logging
{
    internal class LogMessage : ILogMessage
    {
        private readonly string _message;
        private readonly object[] _parameters;

        public LogMessage(IModuleLogger logger, string className, LogLevel level, Exception exception, string message, object[] formatParameters)
            : this(logger, className, level, message, formatParameters)
        {
            IsException = true;
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

            // Build message for internal common logging
            LoggerMessage = ClassName + ": " + Message;

            // Concat exception to message
            if(IsException)
            {
                Message += string.Format("\nException: {0}{1}\nStackTrace: {2}",
                    Exception.Message, Exception.InnerException != null ? "\nInner exception: " + Exception.InnerException.Message : "", Exception.StackTrace);
            }
        }

        #region Internal helper properties
        public ILog TargetLog { get; set; }
        public string LoggerMessage { get; private set; }

        public bool IsException { get; private set; }
        public Exception Exception { get; private set; }
        #endregion

        #region ILogMessage
        /// <summary>
        /// Name of the logger that created the message
        /// </summary>
        public IModuleLogger Logger { get; private set; }

        /// <summary>
        /// Logging class
        /// </summary>
        public string ClassName { get; private set; }

        /// <summary>
        /// Level of the message
        /// </summary>
        public LogLevel Level { get; private set; }

        /// <summary>
        /// The logged message
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// The moment this message was passed to the logger
        /// </summary>
        public DateTime Timestamp { get; private set; }
        #endregion

    }
}
