using System;
using System.Linq;
using Moryx.Runtime.Modules;
#if COMMERCIAL
using Moryx.ControlSystem;
#endif

namespace Moryx.Notifications.Publisher
{
    internal class NotificationPublisherFacade : IFacadeControl, INotificationPublisher
    {
        public Action ValidateHealthState { get; set; }

        public INotificationManager NotificationManager { get; set; }

        public NotificationPublisherFacade()
        {
            ValidateHealthState = InitialValidateHealthState;
        }

        public void Activate()
        {
            NotificationManager.Published += OnPublished;
            NotificationManager.Acknowledged += OnAcknowledged;
        }

        public void Deactivate()
        {
            NotificationManager.Published -= OnPublished;
            NotificationManager.Acknowledged -= OnAcknowledged;
        }

        public Notification[] GetAll()
        {
#if COMMERCIAL
            if (!LicenseCheck.HasLicense())
                return new Notification[0];
#endif
            ValidateHealthState();
            return NotificationManager.GetAll();
        }

        private void OnPublished(object sender, Notification e)
        {
#if COMMERCIAL
            if (!LicenseCheck.HasLicense())
                return;
#endif
            Published?.Invoke(this, e);
        }

        private void OnAcknowledged(object sender, Notification e)
        {
#if COMMERCIAL
            if (!LicenseCheck.HasLicense())
                return;
#endif
            Acknowledged?.Invoke(this, e);          
        }

        public event EventHandler<Notification> Published;

        public event EventHandler<Notification> Acknowledged;

     
        public Notification Get(Guid id)
        {
            return GetAll().Single(n => n.Identifier == id) as Notification;
        }

        public void Acknowledge(Notification notification)
        {
#if COMMERCIAL
            if (!LicenseCheck.HasLicense())
                return;
#endif
            NotificationManager.Acknowledge(notification.Identifier);
        }

        private void InitialValidateHealthState()
        {
            throw new HealthStateException(ServerModuleState.Stopped);
        }
    }
}