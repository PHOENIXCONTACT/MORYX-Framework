// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using Moryx.Logging;
using Microsoft.AspNetCore.Mvc;
using Moryx.Runtime.Endpoints.Logging.Endpoint.Models;
using Moryx.Runtime.Endpoints.Logging.Endpoint.Request;
using System.Collections.Generic;
using Moryx.Runtime.Endpoints.Logging.Endpoint.Response;
using System;
using Moryx.Runtime.Logging;

namespace Moryx.Runtime.Endpoints.Logging.Endpoint
{
    [ApiController]
    [Route("loggers")]
    public class LogController : ControllerBase
    {
        private readonly IServerLoggerManagement _loggerManagement;
        private readonly ILoggingAppender _loggingAppender;

        public LogController(IServerLoggerManagement loggerManagement, ILoggingAppender loggingAppender)
        {
            _loggerManagement = loggerManagement;
            _loggingAppender = loggingAppender;
        }

        [HttpGet]
        public ActionResult<IEnumerable<LoggerModel>> GetAllLoggers()
            => Ok(_loggerManagement.Select(Convert));


        [HttpPut("logger/{loggerName}/loglevel")]
        public ActionResult<InvocationResponse> SetLogLevel([FromRoute] string loggerName, [FromBody] SetLogLevelRequest request)
        {
            var loggers = _loggerManagement.Select(Convert);
            if (!loggers.Any(l => l.Name == loggerName))
                throw new ArgumentException($"No such {loggerName} could not be found");

            _loggerManagement.SetLevel(loggerName, request.Level);
            return new InvocationResponse();
        }

        [HttpPost("appender")]
        public ActionResult<AddAppenderResponse> AddAppender([FromBody] AddAppenderRequest request)
        {
            if (request.Name == null)
                throw new ArgumentNullException(nameof(request.Name));

            return new AddAppenderResponse
            {
                AppenderId = _loggingAppender.AddRemoteLogAppender(request.Name, request.MinLevel)
            };
        }

        [HttpGet("appender/{appenderId}")]
        public ActionResult<IEnumerable<LogMessageModel>> GetMessages([FromRoute] string appenderId)
        {
            var appender = int.Parse(appenderId);
            if (!_loggingAppender.ValidAppender(appender))
                return NotFound($"Appender with Id {appenderId} could not be found");

            var messages = _loggingAppender.FlushMessages(appender).Select(Convert);
            return Ok(messages);
        }

        [HttpDelete("appender/{appenderId}")]
        public ActionResult<InvocationResponse> RemoveAppender([FromRoute] string appenderId)
        {
            var appender = int.Parse(appenderId);
            if (!_loggingAppender.ValidAppender(appender))
                throw new ArgumentException($"Appender with Id {appenderId} could not be found");

            _loggingAppender.RemoveRemoteLogAppender(appender);
            return new InvocationResponse();
        }

        private LogMessageModel Convert(ILogMessage message)
        {
            return new LogMessageModel
            {
                Logger = Convert(message.Logger),
                ClassName = message.ClassName,
                LogLevel = message.Level,
                Message = message.Message,
                Timestamp = message.Timestamp
            };
        }

        private LoggerModel Convert(IModuleLogger logger)
            => Convert(logger, true);

        private LoggerModel Convert(IModuleLogger logger, bool withChildren)
        {
            return new LoggerModel
            {
                Name = logger.Name,
                ActiveLevel = logger.ActiveLevel,
                ChildLogger = withChildren ? logger.Select(Convert).ToArray() : Array.Empty<LoggerModel>(),
                Parent = logger.Parent == null ? null : Convert(logger.Parent, false)
            };
        }
    }
}
