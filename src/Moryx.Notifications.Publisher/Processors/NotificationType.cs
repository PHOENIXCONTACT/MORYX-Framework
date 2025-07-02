namespace Moryx.Notifications.Publisher
{
    /// <summary>
    /// Implementation of the <see cref="INotificationType"/>
    /// </summary>
    public class NotificationType : INotificationType
    {
        /// <summary>
        /// Unique id of the notification (normally the database id)
        /// </summary>
        public long Id { get; set; }

        /// <inheritdoc />
        public string Type { get; set; }

        /// <inheritdoc />
        public virtual string Identifier { get; set; }

        /// <inheritdoc />
        public Severity Severity { get; set; }

        /// <inheritdoc />
        public bool IsDisabled { get; set; }
    }
}