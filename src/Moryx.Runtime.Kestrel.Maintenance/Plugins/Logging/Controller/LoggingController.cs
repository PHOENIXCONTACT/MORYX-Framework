using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Moryx.Container;
using Moryx.Logging;

namespace Moryx.Runtime.Kestrel.Maintenance
{
    /// <summary>
    /// Controller logger
    /// </summary>
    [ApiController]
    [Route("loggers")]
    [ServiceName("ILogMaintenance")]
    internal class LoggingController : Microsoft.AspNetCore.Mvc.Controller
    {
        public IServerLoggerManagement LoggerManagement { get; set; }

        public ILoggingAppender LoggingAppender { get; set; }

        /// <summary>
        /// Logger of this component
        /// </summary>
        [UseChild("KestrelService")]
        public IModuleLogger Logger { get; set; }

        /// <summary>
        /// Get all plugin logger.
        /// </summary>
        /// <returns>Array of plugin logger.</returns>
        [HttpGet]
        [Produces("application/json")]
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
                Parent = logger.Parent == null ? null : Convert(logger.Parent, false)
            };
            return loggerModel;
        }

        /// <summary>
        /// Add a remote appender to the logging stream.
        /// </summary>
        /// <returns>Id to identify the appender.</returns>
        [HttpPost("appender")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public AddAppenderResponse AddAppender([FromBody] AddAppenderRequest request)
        {
            return new AddAppenderResponse
            {
                AppenderId = LoggingAppender.AddRemoteLogAppender(request.Name, request.MinLevel)
            };
        }

        /// <summary>
        /// Get the messages of the appender.
        /// </summary>
        /// <param name="appenderId">The id of the appender.</param>
        /// <returns>Log messages for the appender.</returns>
        [HttpGet("appender/{appenderId}")]
        [Produces("application/json")]
        public ActionResult<LogMessageModel[]> GetMessages(string appenderId)
        {
            var appender = int.Parse(appenderId);
            if (!LoggingAppender.ValidAppender(appender))
            {
                return NotFound(new LogMessageModel[0]);
            }

            var messages = LoggingAppender.FlushMessages(appender).Select(Convert);
            return Ok(messages.ToArray());
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

        /// <summary>
        /// Removes a remote appender from the logging stream.
        /// </summary>
        /// <param name="appenderId">The id of the appender.</param>
        [HttpDelete("appender/{appenderId}")]
        [Produces("application/json")]
        public InvocationResponse RemoveAppender(string appenderId)
        {
            var appender = int.Parse(appenderId);
            LoggingAppender.RemoveRemoteLogAppender(appender);

            return new InvocationResponse();
        }

        /// <summary>
        /// Set the log level of aa logger.
        /// </summary>
        /// <param name="loggerName">Name of the logger</param>
        /// <param name="request"></param>
        [HttpPut("logger/{loggerName}/loglevel")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public InvocationResponse SetLogLevel(string loggerName, [FromBody] SetLogLevelRequest request)
        {
            LoggerManagement.SetLevel(loggerName, request.Level);
            return new InvocationResponse();
        }
    }
}
