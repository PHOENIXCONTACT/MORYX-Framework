// Copyright (c) 2022, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using System;

namespace Moryx.Logging
{
    /// <summary>
    ///Slim wrapper around <see cref="ILogger"/> for module focused logging
    /// </summary>
    public class ModuleLogger : IModuleLogger
    {
        private ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;

        private string _hostName;

        public string Name { get; }

        protected Action<LogLevel, string, Exception> NotificationTarget { get; set; }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }

        IDisposable ILogger.BeginScope<TState>(TState state)
        {
            return _logger.BeginScope(state);
        }

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

        public void SetNotificationTarget(Action<LogLevel, string, Exception> notificationTarget)
        {
            NotificationTarget = notificationTarget;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _logger.Log(logLevel, eventId, state, exception, formatter);
        }
    }
}
