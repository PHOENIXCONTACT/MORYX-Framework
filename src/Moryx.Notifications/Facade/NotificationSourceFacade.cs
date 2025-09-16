// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Moryx.Runtime.Modules;

namespace Moryx.Notifications
{
    /// <summary>
    /// Facade implementation which uses the <see cref="INotificationSourceAdapter"/> to
    /// provide the facade api <see cref="INotificationSource"/>
    /// </summary>
    [DebuggerDisplay("NotificationSource: {" + nameof(Name) + "}")]
    public class NotificationSourceFacade : FacadeBase, INotificationSource
    {
        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        /// Creates a new named notification source facade
        /// </summary>
        /// <summary>
        /// Constructor to create a new instance of <see cref="NotificationSourceFacade"/>
        /// </summary>
        /// <param name="name"></param>
        public NotificationSourceFacade(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The Adapter to interact with the notification senders
        /// </summary>
        public INotificationSourceAdapter NotificationAdapter { get; set; }

        /// <inheritdoc />
        public override void Activate()
        {
            base.Activate();

            NotificationAdapter.Published += OnNotificationPublished;
            NotificationAdapter.Acknowledged += OnNotificationAcknowledged;
        }

        /// <inheritdoc />
        public override void Deactivate()
        {
            NotificationAdapter.Published -= OnNotificationPublished;
            NotificationAdapter.Acknowledged -= OnNotificationAcknowledged;

            base.Deactivate();
        }

        /// <inheritdoc />
        public IReadOnlyList<Notification> GetPublished()
        {
            ValidateHealthState();
            return NotificationAdapter.GetPublished();
        }

        /// <inheritdoc />
        public void Sync()
        {
            ValidateHealthState();
            NotificationAdapter.Sync();
        }

        /// <inheritdoc />
        public void Acknowledge(Notification notification)
        {
            // No ValidateHealthState: Source published the notification; it must be able to handle a response, too!
            NotificationAdapter.Acknowledge(notification);
        }

        /// <inheritdoc />
        public void PublishProcessed(Notification notification)
        {
            // No ValidateHealthState: Source published the notification; it must be able to handle a response, too!
            NotificationAdapter.PublishProcessed(notification);
        }

        /// <inheritdoc />
        public void AcknowledgeProcessed(Notification notification)
        {
            // No ValidateHealthState: Source acknowledge the notification; it must be able to handle a response, too!
            NotificationAdapter.AcknowledgeProcessed(notification);
        }

        private void OnNotificationPublished(object sender, Notification notification)
        {
            // Add this facade to the notification to find it on restore
            notification.Source = Name;

            Published?.Invoke(this, notification);
        }

        private void OnNotificationAcknowledged(object sender, Notification notification)
        {
            Acknowledged?.Invoke(this, notification);
        }

        /// <inheritdoc />
        public event EventHandler<Notification> Published;

        /// <inheritdoc />
        public event EventHandler<Notification> Acknowledged;
    }
}
