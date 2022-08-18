// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moryx.Container;
using Moryx.Modules;

namespace Moryx.Logging
{
    /// <summary>
    /// Logger instance used within one module. All entries will be logged in the modules context.
    /// </summary>
    public interface IModuleLogger : ILogger, INamedChildContainer<IModuleLogger>
    {
        /// <summary>
        /// Name of this logger instance
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Define a target for all messages with a level of <see cref="LogLevel.Warning"/> or higher
        /// </summary>
        void SetNotificationTarget(Action<LogLevel, string, Exception> notificationTarget);
    }
}
