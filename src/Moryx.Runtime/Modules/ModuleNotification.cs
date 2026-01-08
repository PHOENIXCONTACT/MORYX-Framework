// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Modules;
using Moryx.Notifications;

namespace Moryx.Runtime.Modules;

internal class ModuleNotification : IModuleNotification
{
    /// <summary>
    /// Type of this notification
    /// </summary>
    public Severity Severity { get; }

    /// <summary>
    /// Notification message
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Time stamp of occurence
    /// </summary>
    public DateTime Timestamp { get; }

    /// <summary>
    /// Optional exception as cause of this message
    /// </summary>
    public Exception Exception { get; }

    public ModuleNotification(Severity severity, string message, Exception exception)
    {
        Severity = severity;
        Message = message;
        Timestamp = DateTime.Now;
        Exception = exception;
    }

    public static ModuleNotification FromLogStream(LogLevel logLevel, string message, Exception exception)
    {
        var severity = LogLevelToSeverity(logLevel);
        return new ModuleNotification(severity, message, exception);
    }

    private static Severity LogLevelToSeverity(LogLevel logLevel)
    {
        switch (logLevel)
        {
            case LogLevel.Trace:
            case LogLevel.Debug:
            case LogLevel.Information:
                return Severity.Info;
            case LogLevel.Warning:
                return Severity.Warning;
            case LogLevel.Error:
                return Severity.Error;
            case LogLevel.Critical:
                return Severity.Fatal;
            default:
                throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
        }
    }
}

internal static class NotificationCollectionExtension
{
    public static void AddFromLogStream(this INotificationCollection collection, LogLevel logLevel, string message, Exception exception)
    {
        collection.Add(ModuleNotification.FromLogStream(logLevel, message, exception));
    }
}