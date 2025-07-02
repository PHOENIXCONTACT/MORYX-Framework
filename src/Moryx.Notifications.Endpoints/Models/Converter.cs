using Moryx.Serialization;

namespace Moryx.Notifications.Endpoints
{
    internal static class Converter
    {
        private static readonly EntrySerializeSerialization NotificationSerialization = new(typeof(Notification))
        {
            FilterExplicitProperties = true
        };

        internal static NotificationModel ToModel(Notification notification)
        {
            var model = new NotificationModel
            {
                Identifier = notification.Identifier,
                Type = notification.GetType().Name,
                Severity = notification.Severity,
                Title = notification.Title,
                Message = notification.Message,
                
                Acknowledged = notification.Acknowledged,
                IsAcknowledgable = notification.IsAcknowledgable,
                Sender = notification.Sender,
                Source = notification.Source,
                Properties = EntryConvert.EncodeObject(notification, NotificationSerialization)
            };

            if(notification.Created != null)
                model.Created = (System.DateTime)notification.Created;
            return model;
        }
    }
}
