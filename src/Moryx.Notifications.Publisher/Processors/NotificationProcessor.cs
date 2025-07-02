using Moryx.Container;

namespace Moryx.Notifications.Publisher
{
    /// <summary>
    /// Processors for the base notification of type <see cref="Notification"/>
    /// </summary>
    [Plugin(LifeCycle.Singleton, typeof(INotificationProcessor))]
    internal class NotificationProcessor : NotificationProcessorBase<Notification>
    {
    }
}