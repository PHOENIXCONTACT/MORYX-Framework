// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Moryx.Notifications;

namespace Moryx.Modules
{
    /// <summary>
    /// Notification raised by a module. May disappear automatically or has to be acknowledged explicitly
    /// </summary>
    public interface IModuleNotification : INotification
    {
        /// <summary>
        /// Optional exception as cause of this message
        /// </summary>
        Exception Exception { get; }
    }
}
