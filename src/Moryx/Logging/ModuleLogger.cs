// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Moryx.Logging;

/// <summary>
///Slim wrapper around <see cref="ILogger"/> for module focused logging
/// </summary>
public class ModuleLogger : IModuleLogger
{
    private readonly ILogger _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ConcurrentDictionary<string, object> _properties = new();

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
    /// <param name="notificationTarget">
    /// The callback used to forward notifications.
    /// </param>
    public ModuleLogger(string name, ILoggerFactory loggerFactory,
                    Action<LogLevel, string, Exception> notificationTarget)
        : this(name, loggerFactory, loggerFactory.CreateLogger(name), notificationTarget)
    {
    }

    private ModuleLogger(string name, ILoggerFactory loggerFactory,
        ILogger logger, Action<LogLevel,
            string, Exception> notificationTarget)
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
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var enrichedState = new EnrichedState<TState>(state, _properties.ToArray());

        _logger.Log(
            logLevel,
            eventId,
            enrichedState,
            exception,
            (enriched, ex) => formatter(enriched.OriginalState, ex));

        if (logLevel >= LogLevel.Warning)
        {
            NotificationTarget?.Invoke(
                logLevel, formatter(state, exception),
                exception);
        }
    }

    internal void SetProperty(string key, object value)
    {
        _properties[key] = value;
    }

    internal bool RemoveProperty(string key)
    {
        return _properties.TryRemove(key, out _);
    }

    private sealed class EnrichedState<TState>
        : IReadOnlyList<KeyValuePair<string, object>>
    {
        private readonly IReadOnlyList<KeyValuePair<string, object>> _original;
        private readonly IReadOnlyList<KeyValuePair<string, object>> _properties;

        public EnrichedState(TState originalState,
            IReadOnlyList<KeyValuePair<string, object>> properties)
        {
            OriginalState = originalState;

            _original =
                originalState as IReadOnlyList<KeyValuePair<string, object>>
                ?? [];

            _properties = properties;
        }

        public TState OriginalState { get; }

        public int Count => _original.Count + _properties.Count;

        public KeyValuePair<string, object> this[int index]
            =>index < _original.Count
            ? _original[index]
            : _properties[index - _original.Count];

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (var item in _original)
            {
                yield return item;
            }

            foreach (var item in _properties)
            {
                yield return item;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
