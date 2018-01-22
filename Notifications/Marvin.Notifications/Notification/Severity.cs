namespace Marvin.Notifications
{
    /// <summary>
    /// Possible severities of notifications.
    /// </summary>
    public enum Severity
    {
        /// <summary>
        /// The notification is just for informational purpose.
        /// </summary>
        Info,

        /// <summary>
        /// The notification shows a warning.
        /// </summary>
        Warning,

        /// <summary>
        /// The notification shows an error.
        /// </summary>
        Error,

        /// <summary>
        /// The notification shows an fatal error.
        /// </summary>
        Fatal
    }
}