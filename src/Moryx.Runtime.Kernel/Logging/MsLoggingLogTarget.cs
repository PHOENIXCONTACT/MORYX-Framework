// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Logging;
using LogLevel = Moryx.Logging.LogLevel;

namespace Moryx.Runtime.Kernel
{
    internal class MsLoggingLogTarget : ILogTarget
    {
        private readonly ILogger _internalTarget;

        public MsLoggingLogTarget(ILoggerFactory loggerFactory, string name)
        {
            _internalTarget = loggerFactory.CreateLogger(name);
        }

        public void Log(ILogMessage logMessage)
        {
            var logLevel = logMessage.Level switch
            {
                LogLevel.Trace => Microsoft.Extensions.Logging.LogLevel.Trace,
                LogLevel.Debug => Microsoft.Extensions.Logging.LogLevel.Debug,
                LogLevel.Info => Microsoft.Extensions.Logging.LogLevel.Information,
                LogLevel.Warning => Microsoft.Extensions.Logging.LogLevel.Warning,
                LogLevel.Error => Microsoft.Extensions.Logging.LogLevel.Error,
                LogLevel.Fatal => Microsoft.Extensions.Logging.LogLevel.Critical,
                _ => Microsoft.Extensions.Logging.LogLevel.None
            };

            var logEvent = new MsLoggingLogEvent(logMessage.Message);
            logEvent.WithProperty(nameof(logMessage.ClassName), logMessage.ClassName);

            _internalTarget.Log(logLevel, default, logEvent, logMessage.Exception, MsLoggingLogEvent.Formatter);
        }
    }
}
