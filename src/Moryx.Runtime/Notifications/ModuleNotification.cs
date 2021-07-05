// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Modules;
using Moryx.Notifications;

namespace Moryx.Runtime.Notifications
{
    internal class ModuleNotification : Notification, IModuleNotification
    {
        /// <summary>
        /// Optional exception as cause of this message
        /// </summary>
        public Exception Exception { get; set; }
    }
}
