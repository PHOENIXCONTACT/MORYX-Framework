// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

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
