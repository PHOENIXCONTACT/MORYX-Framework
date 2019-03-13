using System;
using System.Collections.Generic;
using Marvin.Container;

namespace Marvin.Logging
{
    /// <summary>
    /// Different levels to diffentiate the severity of a message
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Information simply used to trace the flow of operations inside a module
        /// </summary>
        Trace,

        /// <summary>
        /// Information used for debugging purposes
        /// </summary>
        Debug,

        /// <summary>
        /// Information about occured events that are nether a warning nor an error
        /// </summary>
        Info,

        /// <summary>
        /// Events that may destabilize the component
        /// </summary>
        Warning,

        /// <summary>
        /// Critical events that may obstruct any further execution
        /// </summary>
        Error,

        /// <summary>
        /// Critical event that does prevent any further component usage
        /// </summary>
        Fatal
    }

    /// <summary>
    /// Logger instance used within one module. All entries will be logged in the modules context.
    /// </summary>
    public interface IModuleLogger : IEnumerable<IModuleLogger>, INamedChildContainer<IModuleLogger>, IContainerChild<IModuleLogger>
    {
        /// <summary>
        /// Name of this logger instance
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Add a new entry to the log
        /// </summary>
        void Log(LogLevel level, string message, params object[] formatParameters);

        /// <summary>
        /// Log a caught exception
        /// </summary>
        void LogException(LogLevel level, Exception ex, string message, params object[] formatParameters);

        /// <summary>
        /// Active logging level
        /// </summary>
        LogLevel ActiveLevel { get; }

        /// <summary>
        /// Clone this instance to be used on a new object
        /// </summary>
        IModuleLogger Clone(Type targetType);
    }
}