using System;

namespace Marvin.Notifications
{
    public interface IManagedNotification : INotification
    {
        /// <summary>
        /// Override with setter.
        /// <inheritdoc cref="INotification.Identifier"/>
        /// </summary>
        new string Identifier { get; set; }

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
    public interface INotification : IQuickCast
    {
        /// <summary>
        /// Unique identifier of this notification
        /// </summary>
        string Identifier { get; }

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
    }
}
