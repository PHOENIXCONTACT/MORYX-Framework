using System;

namespace Marvin.Notifications
{
    /// <summary>
    /// Facade interface for providing notifications
    /// </summary>
    public interface INotificationSource : INotificationSourceAdapter
    {
        /// <summary>
        /// Name of the Source which will publish notifications
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Publishes the current state of the facade
        /// </summary>
        bool IsActivated { get; }

        /// <summary>
        /// Event will be raised if the facade was deactivated
        /// </summary>
        event EventHandler<INotification[]> Deactivated;
    }
}