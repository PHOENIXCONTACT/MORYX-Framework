// Copyright (c) 2022, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Logging;
using Moryx.Modules;
using System;
using MoryxLogLevel = Moryx.Logging.LogLevel;
using MsLogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Moryx.Runtime.Logging
{
    internal class ModuleLogger : IModuleLogger
    {
        private ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;

        private string _hostName;

        public string Name { get; }

        protected Action<MoryxLogLevel, string, Exception> NotificationTarget { get; set; }

        public ModuleLogger(string name, Type targetType, ILoggerFactory loggerFactory)
            : this(name, targetType, loggerFactory, loggerFactory.CreateLogger(name))
        {
        }

        private ModuleLogger(string name, Type targetType, ILoggerFactory loggerFactory, ILogger logger)
        {
            Name = name;

            _hostName = targetType?.Name ?? "Unknown";

            _logger = logger;
            _loggerFactory = loggerFactory;
        }

        public IModuleLogger GetChild(string name, Type target)
        {
            var logger = string.IsNullOrEmpty(name)
                ? new ModuleLogger(Name, target, _loggerFactory, _logger)
                : new ModuleLogger($"{Name}.{name}", target, _loggerFactory);

            logger.NotificationTarget = NotificationTarget;

            return logger;
        }

        public void Log(MoryxLogLevel level, string message, params object[] formatParameters)
        {
            Log(level, null, message, formatParameters);
        }

        public void LogException(MoryxLogLevel level, Exception ex, string message, params object[] formatParameters)
        {
            var logLevel = level switch
            {
                MoryxLogLevel.Trace => MsLogLevel.Trace,
                MoryxLogLevel.Debug => MsLogLevel.Debug,
                MoryxLogLevel.Info => MsLogLevel.Information,
                MoryxLogLevel.Warning => MsLogLevel.Warning,
                MoryxLogLevel.Error => MsLogLevel.Error,
                MoryxLogLevel.Fatal => MsLogLevel.Critical,
                _ => MsLogLevel.None
            };

            var logEvent = new ModuleLoggerEvent(message, formatParameters);
            logEvent.WithProperty("ClassName", _hostName);

            _logger.Log(logLevel, default, logEvent, ex, ModuleLoggerEvent.Formatter);

            if (level >= MoryxLogLevel.Warning)
                NotificationTarget(level, ModuleLoggerEvent.Formatter(logEvent, ex), ex);
        }

        public void SetNotificationTarget(Action<MoryxLogLevel, string, Exception> notificationTarget)
        {
            NotificationTarget = notificationTarget;
        }
    }
}
