namespace Marvin.Notifications
{
    /// <summary>
    /// Interface for components which are sending notifications
    /// </summary>
    public interface INotificationSender
    {
        /// <summary>
        /// Name of the notification sender
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Inform the sender about the acknowledged notification
        /// </summary>
        void Acknowledge(INotification notification);
    }
}