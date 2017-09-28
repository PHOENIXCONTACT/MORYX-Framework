using System.ServiceModel;
using Marvin.Logging;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.Maintenance.Plugins.LogMaintenance.Wcf
{
    /// <summary>
    /// Service contract for logging features of the maintenance.
    /// </summary>
    [ServiceContract]
    [ServiceVersion(ServerVersion = "1.0.1.0", MinClientVersion = "1.0.1.0")]
    public interface ILogMaintenance
    {
        /// <summary>
        /// Get all plugin logger.
        /// </summary>
        /// <returns>Array of plugin logger.</returns>
        [OperationContract]
        PluginLoggerModel[] GetAllPluginLogger();

        /// <summary>
        /// Add a remote appender to the logging stream.
        /// </summary>
        /// <param name="name">Name of the appender.</param>
        /// <param name="minLevel">Minimum level to log.</param>
        /// <returns>Id to identify the appender.</returns>
        [OperationContract]
        int AddRemoteAppender(string name, LogLevel minLevel);

        /// <summary>
        /// Get the messages of the appender.
        /// </summary>
        /// <param name="appenderId">The id of the appender.</param>
        /// <returns>Log messages for the appender.</returns>
        [OperationContract]
        LogMessages GetMessages(int appenderId);

        /// <summary>
        /// Removes a remote appender from the loggin stream.
        /// </summary>
        /// <param name="appenderId">The id of the appender.</param>
        [OperationContract]
        void RemoveRemoteAppender(int appenderId);

        /// <summary>
        /// Set the log level of aa logger.
        /// </summary>
        /// <param name="logger">Change the level of this logger.</param>
        /// <param name="level">The level where to change to.</param>
        [OperationContract]
        void SetLogLevel(PluginLoggerModel logger, LogLevel level);
    }
}
