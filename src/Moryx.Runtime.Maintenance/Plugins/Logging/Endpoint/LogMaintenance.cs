// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using System.Net;
using Moryx.Container;
using Moryx.Logging;

#if USE_WCF
using System.ServiceModel;
using System.ServiceModel.Web;
#else
using Microsoft.AspNetCore.Mvc;
using Moryx.Communication.Endpoints;
#endif

namespace Moryx.Runtime.Maintenance.Plugins.Logging
{
    [Plugin(LifeCycle.Transient, typeof(ILogMaintenance))]
#if USE_WCF
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, IncludeExceptionDetailInFaults = true)]
    public class LogMaintenance : ILogMaintenance, ILoggingComponent
#else
    [ApiController, Route(Endpoint), Produces("application/json")]
    [Endpoint(Name = nameof(ILogMaintenance), Version = "3.0.0")]
    public class LogMaintenance : Controller, ILogMaintenance, ILoggingComponent
#endif
    {
        internal const string Endpoint = "loggers";

        #region Dependencies

        public IServerLoggerManagement LoggerManagement { get; set; }

        public ILoggingAppender LoggingAppender { get; set; }

        /// <summary>
        /// Logger of this component
        /// </summary>
        [UseChild("WcfService")]
        public IModuleLogger Logger { get; set; }

        #endregion

        /// <inheritdoc />
#if !USE_WCF
        [HttpGet]
#endif
        public LoggerModel[] GetAllLoggers()
        {
            return LoggerManagement.Select(Convert).ToArray();
        }

        /// <inheritdoc />
#if !USE_WCF
        [HttpPut("logger/{loggerName}/loglevel")]
#endif
        public InvocationResponse SetLogLevel(string loggerName, SetLogLevelRequest request)
        {
            LoggerManagement.SetLevel(loggerName, request.Level);
            return new InvocationResponse();
        }

        /// <inheritdoc />
#if !USE_WCF
        [HttpPost("appender")]
#endif
        public AddAppenderResponse AddAppender(AddAppenderRequest request)
        {
            return new AddAppenderResponse
            {
                AppenderId = LoggingAppender.AddRemoteLogAppender(request.Name, request.MinLevel)
            };
        }

        /// <inheritdoc />
#if !USE_WCF
        [HttpGet("appender/{appenderId}")]
#endif
        public LogMessageModel[] GetMessages(string appenderId)
        {
            var appender = int.Parse(appenderId);
            if (!LoggingAppender.ValidAppender(appender))
            {
#if USE_WCF
                var ctx = WebOperationContext.Current;
                // ReSharper disable once PossibleNullReferenceException
                ctx.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
#else
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
#endif
                return new LogMessageModel[0];
            }

            var messages = LoggingAppender.FlushMessages(appender).Select(Convert);
            return messages.ToArray();
        }

        /// <inheritdoc />
#if !USE_WCF
        [HttpDelete("appender/{appenderId}")]
#endif
        public InvocationResponse RemoveAppender(string appenderId)
        {
            var appender = int.Parse(appenderId);
            LoggingAppender.RemoveRemoteLogAppender(appender);

            return new InvocationResponse();
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
                Parent = logger.Parent == null ? null : Convert(logger.Parent, false)
            };
            return loggerModel;
        }
    }
}
