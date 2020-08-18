// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using System.Net;
using System.ServiceModel;
using Moryx.Container;
using Moryx.Logging;

namespace Moryx.Runtime.Maintenance.Plugins.Logging
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

        public LoggerModel[] GetAllLoggers()
        {
            return LoggerManagement.Select(Convert).ToArray();
        }

        private LoggerModel Convert(IModuleLogger logger)
        {
            return Convert(logger, true);
        }

        private LoggerModel Convert(IModuleLogger logger, bool withChildren)
        {
            var loggerModel = new LoggerModel
            {
                Name = logger.Name,
                ActiveLevel = logger.ActiveLevel,
                ChildLogger = withChildren ? logger.Select(Convert).ToArray() : new LoggerModel[0],
                Parent = logger.Parent == null? null : Convert(logger.Parent, false)
            };
            return loggerModel;
        }

        public AddAppenderResponse AddAppender(AddAppenderRequest request)
        {
            return new AddAppenderResponse
            {
                AppenderId = LoggingAppender.AddRemoteLogAppender(request.Name, request.MinLevel)
            };
        }

        public LogMessageModel[] GetMessages(string appenderId)
        {
            var appender = int.Parse(appenderId);
            if (!LoggingAppender.ValidAppender(appender))
            {
                HttpHelper.SetStatusCode(HttpStatusCode.NotFound);
                return new LogMessageModel[0];
            }

            var messages = LoggingAppender.FlushMessages(appender).Select(Convert);
            return messages.ToArray();
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

        public InvocationResponse RemoveAppender(string appenderId)
        {
            var appender = int.Parse(appenderId);
            LoggingAppender.RemoveRemoteLogAppender(appender);

            return new InvocationResponse();
        }

        public InvocationResponse SetLogLevel(string loggerName, SetLogLevelRequest request)
        {
            LoggerManagement.SetLevel(loggerName, request.Level);
            return new InvocationResponse();
        }

        
    }
}
