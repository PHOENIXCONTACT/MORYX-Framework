namespace Marvin.Runtime.Maintenance.Plugins.Logging
{
    /// <summary>
    /// Response for adding a logging appender
    /// </summary>
    public class AddAppenderResponse
    {
        /// <summary>
        /// Id of the logging appender
        /// </summary>
        public int AppenderId { get; set; }
    }
}