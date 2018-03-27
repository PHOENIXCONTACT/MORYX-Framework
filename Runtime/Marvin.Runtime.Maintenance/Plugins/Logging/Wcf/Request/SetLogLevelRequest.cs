using Marvin.Logging;

namespace Marvin.Runtime.Maintenance.Plugins.Logging
{
    /// <summary>
    /// Request for setting a log level
    /// </summary>
    public class SetLogLevelRequest
    {
        /// <summary>
        /// Log level to set
        /// </summary>
        public LogLevel Level { get; set; }
    }
}
