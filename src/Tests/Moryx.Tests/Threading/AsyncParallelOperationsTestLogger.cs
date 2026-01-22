// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Moryx.Tests.Threading;

internal class AsyncParallelOperationsTestLogger : ILogger
{
    private readonly List<(LogLevel level, string message, Exception exception)> _logs = new();

    public Exception LastException =>
        _logs.LastOrDefault().exception;

    public LogLevel LastLogLevel =>
        _logs.LastOrDefault().level;

    public bool HasErrors =>
        _logs.Any(l => l.level == LogLevel.Error);

    public bool HasCriticalErrors =>
        _logs.Any(l => l.level == LogLevel.Critical);

    public int ErrorCount =>
        _logs.Count(l => l.level == LogLevel.Error || l.level == LogLevel.Critical);

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
        Func<TState, Exception, string> formatter)
    {
        var message = formatter?.Invoke(state, exception) ?? state?.ToString();

        lock (_logs)
        {
            _logs.Add((logLevel, message, exception));
        }
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable BeginScope<TState>(TState state) => null;
}
