using System.Linq;
using System.ServiceModel;
using Marvin.Container;
using Marvin.Logging;

namespace Marvin.Runtime.Maintenance.Plugins.LogMaintenance.Wcf
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, IncludeExceptionDetailInFaults = true)]
    [Plugin(LifeCycle.Transient, typeof(ILogMaintenance))]
    internal class LogMaintenance : ILogMaintenance, ILoggingComponent
    {
        public IServerLoggerManagement LoggerManagement { get; set; }
        public ILoggingAppender LoggingAppender { get; set; }

        /// <summary>
        /// Logger of this component
        /// </summary>
        [UseChild("WcfService")]
        public IModuleLogger Logger { get; set; }

        public PluginLoggerModel[] GetAllPluginLogger()
        {
            return LoggerManagement.Select(Convert).ToArray();
        }

        private PluginLoggerModel Convert(IModuleLogger logger)
        {
            return Convert(logger, true);
        }

        private PluginLoggerModel Convert(IModuleLogger logger, bool withChildren)
        {
            var loggerModel = new PluginLoggerModel
            {
                Name = logger.Name,
                ActiveLevel = logger.ActiveLevel,
                ChildLogger = withChildren ? logger.Select(Convert).ToArray() : new PluginLoggerModel[0],
                Parent = logger.Parent == null? null : Convert(logger.Parent, false)
            };
            return loggerModel;
        }

        public int AddRemoteAppender(string name, LogLevel minLevel)
        {
            return LoggingAppender.AddRemoteLogAppender(name, minLevel);
        }

        public LogMessages GetMessages(int appenderId)
        {
            var result = new LogMessages
            {
                AppenderId = appenderId,
                Successful = LoggingAppender.ValidAppender(appenderId),
                Messages = LoggingAppender.FlushMessages(appenderId).Select(Convert).ToArray()
            };
            return result;
        }

        private LogMessageModel Convert(ILogMessage message)
        {
            var messageModel = new LogMessageModel
            {
                Logger = Convert(message.Logger),
                ClassName = message.ClassName,
                LogLevel = message.Level,
                Message = message.Message,
                Timestamp = message.Timestamp
            };
            return messageModel;
        }

        public void RemoveRemoteAppender(int appenderId)
        {
            LoggingAppender.RemoveRemoteLogAppender(appenderId);
        }

        public void SetLogLevel(PluginLoggerModel loggerModel, LogLevel level)
        {
            LoggerManagement.SetLevel(loggerModel.Name, level);
        }
    }
}