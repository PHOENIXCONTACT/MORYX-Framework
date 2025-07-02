#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Moryx.Model;

namespace Moryx.Notifications.Model
{
    public class NotificationEntity : EntityBase
    {
        public virtual Guid Identifier { get; set; }

        public virtual string Source { get; set; }

        public virtual string Sender { get; set; }

        public virtual string Title { get; set; }

        public virtual string Message { get; set; }

        public virtual DateTime? Acknowledged { get; set; }

        public virtual string ExtensionData { get; set; }

        public virtual DateTime Created { get; set; }

        [ForeignKey(nameof(Type))]
        public virtual long TypeId { get; set; }

        public virtual NotificationTypeEntity Type { get; set; }
    }
}
