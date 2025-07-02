namespace Moryx.Notifications.Publisher
{
    /// <summary>
    /// Result of the processing and acknowledge of the notification processor
    /// </summary>
    public enum NotificationProcessorResult
    {
        /// <summary>
        /// Notification was processed successfully
        /// </summary>
        Processed,

        /// <summary>
        /// Notification was ignored by the processor
        /// </summary>
        Ignored
    }
}