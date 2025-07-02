#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Moryx.Model.Repositories;

namespace Moryx.Notifications.Model
{
    public interface INotificationTypeEntityRepository : IRepository<NotificationTypeEntity>
    {
        NotificationTypeEntity Create(string type, int severity);
    }
}
