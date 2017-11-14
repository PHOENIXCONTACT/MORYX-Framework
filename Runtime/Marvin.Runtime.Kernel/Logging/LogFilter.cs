using Marvin.Logging;

namespace Marvin.Runtime.Kernel.Logging
{
    internal enum FilterType
    {
        /// <summary>
        /// No filtering options
        /// </summary>
        None = 0,

        /// <summary>
        /// Filter by name
        /// </summary>
        NameBased = 1,

        /// <summary>
        /// Filter by level
        /// </summary>
        LevelBased = 2,

        /// <summary>
        /// Filter by name and level
        /// </summary>
        NameAndLevel = 3
    }

    internal class LogFilter
    {
        /// <summary>
        /// Type of filter
        /// </summary>
        public FilterType FilterType { get; set; }

        /// <summary>
        /// Name of logger to listen to
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Minimum level to listen to
        /// </summary>
        public LogLevel MinLevel { get; set; }
    }
}