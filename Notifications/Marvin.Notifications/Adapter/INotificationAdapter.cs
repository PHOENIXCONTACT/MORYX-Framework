using System.Collections.Generic;

namespace Marvin.Notifications
{
    public interface INotificationAdapter
    {
        /// <summary>
        /// Registers the sender at the adapter
        /// </summary>
        /// <param name="sender"></param>
        INotificationContext Register(INotificationSender sender);

        /// <summary>
        /// Removes the registration of the sender
        /// </summary>
        /// <param name="sender"></param>
        void Unregister(INotificationSender sender);
    }
}