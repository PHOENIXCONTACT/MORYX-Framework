using Marvin.Container;

namespace Marvin.Notifications
{
    public static class NotificationExtensions
    {
        public static IContainer RegisterNotifications(this IContainer container)
        {
            var adapter = new NotificationAdapter();

            container.SetInstance((INotificationAdapter)adapter, "NotificationAdapter");
            container.SetInstance((INotificationSenderAdapter)adapter, "NotificationSenderAdapter");

            return container;
        }
    }
}