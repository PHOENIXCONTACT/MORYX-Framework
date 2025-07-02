#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using Moryx.Model.Repositories;

namespace Moryx.Notifications.Model
{
    public interface INotificationEntityRepository : IRepository<NotificationEntity>
    {
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        NotificationEntity Create(Guid identifier, string source, string sender);
    }
}
