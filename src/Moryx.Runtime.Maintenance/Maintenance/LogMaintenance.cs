// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Runtime.Maintenance.Contracts;
using Moryx.Runtime.Maintenance.Logging;
using Nancy;
using Nancy.ModelBinding;

namespace Moryx.Runtime.Maintenance
{
    [Plugin(LifeCycle.Singleton, typeof(INancyModule), typeof(LogMaintenance))]
    internal sealed class LogMaintenance : NancyModule, ILoggingComponent
    {
        public IServerLoggerManagement LoggerManagement { get; set; }

        public ILoggingAppender LoggingAppender { get; set; }

        /// <summary>
        /// Logger of this component
        /// </summary>
        [UseChild("WcfService")]
        public IModuleLogger Logger { get; set; }

        public LogMaintenance() : base("loggers")
        {
            Get("/", args => Response.AsJson(GetAllLoggers()));

            Post("{loggerName}/logLevel", delegate (dynamic args)
            {
                var request = this.Bind<SetLogLevelRequest>();
                var response = SetLogLevel((string)args.moduleName, request);
                return Response.AsJson(response);
            });

            Put("appender", delegate (dynamic args)
            {
                var request = this.Bind<AddAppenderRequest>();
                var response = AddAppender(request);
                return Response.AsJson(response);
            });

            Delete("appender/{appenderId}", delegate (dynamic args)
            {
                var response = RemoveAppender((string)args.appenderId);
                return Response.AsJson(response);
            });

            Get("appender/{appenderId}", args => Response.AsJson(GetMessages((string)args.appenderId)));
        }

        private LoggerModel[] GetAllLoggers()
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

        private AddAppenderResponse AddAppender(AddAppenderRequest request)
        {
            return new AddAppenderResponse
            {
                AppenderId = LoggingAppender.AddRemoteLogAppender(request.Name, request.MinLevel)
            };
        }

        private LogMessageModel[] GetMessages(string appenderId)
        {
            var appender = int.Parse(appenderId);
            if (!LoggingAppender.ValidAppender(appender))
            {
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

        private InvocationResponse RemoveAppender(string appenderId)
        {
            var appender = int.Parse(appenderId);
            LoggingAppender.RemoveRemoteLogAppender(appender);

            return new InvocationResponse();
        }

        private InvocationResponse SetLogLevel(string loggerName, SetLogLevelRequest request)
        {
            LoggerManagement.SetLevel(loggerName, request.Level);
            return new InvocationResponse();
        }
    }
}
