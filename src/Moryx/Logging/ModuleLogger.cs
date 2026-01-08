// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;

namespace Moryx.Logging;

/// <summary>
///Slim wrapper around <see cref="ILogger"/> for module focused logging
/// </summary>
public class ModuleLogger : IModuleLogger
{
    private ILogger _logger;
    private readonly ILoggerFactory _loggerFactory;

    public string Name { get; }

    protected Action<LogLevel, string, Exception> NotificationTarget { get; }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _logger.IsEnabled(logLevel);
    }

    IDisposable ILogger.BeginScope<TState>(TState state)
    {
        return _logger.BeginScope(state);
    }

    public ModuleLogger(string name, ILoggerFactory loggerFactory)
        : this(name, loggerFactory, loggerFactory.CreateLogger(name), null)
    {
    }

    public ModuleLogger(string name, ILoggerFactory loggerFactory, Action<LogLevel, string, Exception> notificationTarget)
        : this(name, loggerFactory, loggerFactory.CreateLogger(name), notificationTarget)
    {
    }

    private ModuleLogger(string name, ILoggerFactory loggerFactory, ILogger logger, Action<LogLevel, string, Exception> notificationTarget)
    {
        Name = name;
        NotificationTarget = notificationTarget;

        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    public IModuleLogger GetChild(string name, Type target)
    {
        var logger = string.IsNullOrEmpty(name)
            ? new ModuleLogger(Name, _loggerFactory, _logger, NotificationTarget)
            : new ModuleLogger($"{Name}.{name}", _loggerFactory, NotificationTarget);
        return logger;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        _logger.Log(logLevel, eventId, state, exception, formatter);

        if (logLevel >= LogLevel.Warning)
            NotificationTarget?.Invoke(logLevel, formatter(state, exception), exception);
    }
}