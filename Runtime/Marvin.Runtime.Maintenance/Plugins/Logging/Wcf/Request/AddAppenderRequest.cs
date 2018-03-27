using Marvin.Logging;

namespace Marvin.Runtime.Maintenance.Plugins.Logging
{
    /// <summary>
    /// Request to add a logging appender
    /// </summary>
    public class AddAppenderRequest
    {
        /// <summary>
        /// Name of the logger
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Minimal log level for the appender
        /// </summary>
        public LogLevel MinLevel { get; set; }
    }
}
