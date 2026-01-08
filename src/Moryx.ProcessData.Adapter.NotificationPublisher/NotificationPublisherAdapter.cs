// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Modules;
using Moryx.Notifications;
using Moryx.ProcessData.Bindings;

namespace Moryx.ProcessData.Adapter.NotificationPublisher
{
    [Plugin(LifeCycle.Singleton)]
    internal class NotificationPublisherAdapter : IPlugin
    {
        private const string MeasurementPrefix = "notifications_";
        private MeasurementBindingProcessor _notificationBindingProcessor;

        #region Dependencies

        public INotificationPublisher NotificationPublisher { get; set; }

        public IProcessDataMonitor ProcessDataMonitor { get; set; }

        public ModuleConfig ModuleConfig { get; set; }

        #endregion

        /// <inheritdoc />
        public void Start()
        {
            var notificationBindingResolverFactory = new NotificationBindingResolverFactory();
            _notificationBindingProcessor = new MeasurementBindingProcessor(notificationBindingResolverFactory, ModuleConfig.NotificationBindings);

            NotificationPublisher.Acknowledged += OnNotificationAcknowledged;
        }

        /// <summary>
        /// Stops the adapter component
        /// </summary>
        public void Stop()
        {
            NotificationPublisher.Acknowledged -= OnNotificationAcknowledged;
        }

        private void OnNotificationAcknowledged(object sender, Notification notification)
        {
            var measurement = new Measurement(MeasurementPrefix + "acknowledged");

            measurement.Add(new DataField("id", notification.Identifier));

            var duration = notification.Acknowledged - notification.Created;
            if (duration.HasValue)
                measurement.Add(new DataField("duration", duration.Value));

            measurement.Add(new DataTag("severity", notification.Severity.ToString()));
            measurement.Add(new DataTag("type", notification.GetType().Name));

            _notificationBindingProcessor.Apply(measurement, notification);

            ProcessDataMonitor.Add(measurement);
        }
    }
}
