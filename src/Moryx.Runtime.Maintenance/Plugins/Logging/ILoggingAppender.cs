// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using Moryx.Logging;
using Moryx.Runtime.Maintenance.Contracts;

namespace Moryx.Runtime.Maintenance.Plugins.Logging
{
    internal interface ILoggingAppender : IMaintenancePlugin
    {
        /// <summary>
        /// Add a remote appender to the logging stream
        /// </summary>
        int AddRemoteLogAppender(string name, LogLevel level);

        /// <summary>
        /// Check if id belongs to a valid appender
        /// </summary>
        bool ValidAppender(int appenderId);

        /// <summary>
        /// Flush all new messages of this appender
        /// </summary>
        IEnumerable<ILogMessage> FlushMessages(int appender);

        /// <summary>
        /// Remove a remote appender from the logging stream
        /// </summary>
        /// <param name="appenderId"></param>
        void RemoveRemoteLogAppender(int appenderId);
    }
}
