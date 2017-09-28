using System.Collections.Generic;
using Marvin.Logging;
using Marvin.Runtime.Maintenance.Contracts;

namespace Marvin.Runtime.Maintenance.Plugins.LogMaintenance
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
