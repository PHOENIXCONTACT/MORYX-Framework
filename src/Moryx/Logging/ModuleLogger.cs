// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;

namespace Moryx.Logging;

/// <summary>
///Slim wrapper around <see cref="ILogger"/> for module focused logging
/// </summary>
public class ModuleLogger : IModuleLogger
{
    private readonly ILogger _logger;
    private readonly ILoggerFactory _loggerFactory;
    private Dictionary<string, object> _scope;

    /// <inheritdoc />
    public string Name { get; }

    /// <summary>
    /// Gets the callback used to forward module notifications.
    /// </summary>
    protected Action<LogLevel, string, Exception> NotificationTarget { get; }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel)
    {
        return _logger.IsEnabled(logLevel);
    }

    IDisposable ILogger.BeginScope<TState>(TState state)
    {
        return _logger.BeginScope(state);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleLogger"/> class.
    /// </summary>
    /// <param name="name">This logger name.</param>
    /// <param name="loggerFactory">The factory used to create the logger.</param>
    public ModuleLogger(string name, ILoggerFactory loggerFactory)
        : this(name, loggerFactory, loggerFactory.CreateLogger(name), null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleLogger"/> class
    /// with a notification target.
    /// </summary>
    /// <param name="name">The logger name.</param>
    /// <param name="loggerFactory">The factory used to create the logger.</param>
    /// <param name="notificationTarget">The function used to forward notifications.</param>
    public ModuleLogger(string name, ILoggerFactory loggerFactory,
        Action<LogLevel, string, Exception> notificationTarget)
        : this(name, loggerFactory, loggerFactory.CreateLogger(name), notificationTarget)
    {
    }

    private ModuleLogger(string name, ILoggerFactory loggerFactory,
        ILogger logger, Action<LogLevel, string, Exception> notificationTarget)
    {
        Name = name;
        NotificationTarget = notificationTarget;

        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public IModuleLogger GetChild(string name, Type target)
    {
        var logger = string.IsNullOrEmpty(name)
            ? new ModuleLogger(Name, _loggerFactory, _logger, NotificationTarget)
            : new ModuleLogger($"{Name}.{name}", _loggerFactory, NotificationTarget);
        return logger;
    }

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
        Func<TState, Exception, string> formatter)
    {
        if (_scope is null)
        {
            _logger.Log(logLevel, eventId, state, exception, formatter);
        }
        else
        {
            using (_logger.BeginScope(_scope))
            {
                _logger.Log(logLevel, eventId, state, exception, formatter);
            }
        }

        if (logLevel >= LogLevel.Warning)
        {
            NotificationTarget?.Invoke(logLevel, formatter(state, exception), exception);
        }
    }

    /// <summary>
    /// Sets the scope properties for log entries created by this logger.
    /// </summary>
    /// <param name="scope">The properties to include in the logging scope.</param>
    public void SetScope(Dictionary<string, object> scope)
    {
        _scope = scope;
    }
}
