using System;

namespace Marvin.Notifications
{
    /// <summary>
    /// Additional notification interface to manipulate the notification properties in managing plugins
    /// </summary>
    public interface IManagedNotification : INotification
    {
        /// <summary>
        /// Override with setter.
        /// <inheritdoc cref="INotification.Identifier"/>
        /// </summary>
        new Guid Identifier { get; set; }

        /// <summary>
        /// Override with setter.
        /// <inheritdoc cref="INotification.Title"/>
        /// </summary>
        new string Title { get; set; }

        /// <summary>
        /// Override with setter.
        /// <inheritdoc cref="INotification.Message"/>
        /// </summary>
        new string Message { get; set; }

        /// <summary>
        /// Override with setter.
        /// <inheritdoc cref="INotification.Severity"/>
        /// </summary>
        new Severity Severity { get; set; }

        /// <summary>
        /// Override with setter.
        /// <inheritdoc cref="INotification.Created"/>
        /// </summary>
        new DateTime Created { get; set; }

        /// <summary>
        /// Override with setter.
        /// <inheritdoc cref="INotification.Acknowledged"/>
        /// </summary>
        new DateTime? Acknowledged { get; set; }

        /// <summary>
        /// Override with setter.
        /// <inheritdoc cref="INotification.IsAcknowledgable"/>
        /// </summary>
        new bool IsAcknowledgable { get; set; }

        /// <summary>
        /// Override with setter.
        /// <inheritdoc cref="INotification.Acknowledger"/>
        /// </summary>
        new string Acknowledger { get; set; }

        /// <summary>
        /// Sender of this notification. <see cref="INotificationSender"/>
        /// </summary>
        string Sender { get; set; }

        /// <summary>
        /// Source of this notification. <see cref="INotificationSource"/>
        /// </summary>
        string Source { get; set; }
    }

    /// <summary>
    /// Message for a consumer
    /// </summary>
    public interface INotification
    {
        /// <summary>
        /// Unique identifier of this notification
        /// </summary>
        Guid Identifier { get; }

        /// <summary>
        /// The severity of this notification
        /// </summary>
        Severity Severity { get; }

        /// <summary>
        /// Optional title of this notification. Can be set by processor as well
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Message of this notification.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Date of creation
        /// </summary>
        DateTime Created { get; }

        /// <summary>
        /// If null, the notification was not acknowledged.
        /// If not null, the notification was already acknowledged
        /// </summary>
        DateTime? Acknowledged { get; }

        /// <summary>
        /// Indicates is the notification can be acknowledged
        /// </summary>
        bool IsAcknowledgable { get; }

        /// <summary>
        /// Who or what acknowledged the notification, if it was acknowledged.
        /// <see cref="Acknowledged"/> shows if the notification has been acknowledged.
        /// </summary>
        string Acknowledger { get; }
    }
}
