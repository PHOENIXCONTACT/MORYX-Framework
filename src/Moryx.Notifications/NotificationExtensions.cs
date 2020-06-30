using Moryx.Container;

namespace Moryx.Notifications
{
    /// <summary>
    /// <see cref="IContainer"/> extensions to register and handle notifications
    /// </summary>
    public static class NotificationExtensions
    {
        /// <summary>
        /// Registers all components to handling notifications in the current container
        /// </summary>
        public static IContainer RegisterNotifications(this IContainer container)
        {
            var adapter = new NotificationAdapter();

            container.SetInstance((INotificationAdapter)adapter, "NotificationAdapter");
            container.SetInstance((INotificationSourceAdapter)adapter, "NotificationSenderAdapter");

            return container;
        }
    }
}