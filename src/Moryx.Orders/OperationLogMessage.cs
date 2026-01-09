// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;

namespace Moryx.Orders;

/// <summary>
/// Cached create step log message from the <see cref="IOperationLogger"/>
/// </summary>
public class OperationLogMessage
{
    /// <summary>
    /// Severity level of the log message
    /// </summary>
    public LogLevel LogLevel { get; set; }

    /// <summary>
    /// A sequence of words to describe the log
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// If available, an exception
    /// </summary>
    public Exception Exception { get; set; }

    /// <summary>
    /// Date time where the log message was occurred
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Constructor to create a log message
    /// </summary>
    public OperationLogMessage(LogLevel logLevel, string message)
    {
        LogLevel = logLevel;
        Message = message;
        Timestamp = DateTime.Now;
    }

    /// <summary>
    /// Constructor to create a log message with an exception
    /// </summary>
    public OperationLogMessage(LogLevel logLevel, Exception exception, string message) : this(logLevel, message)
    {
        Exception = exception;
    }
}