// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
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

        public void Log(LogLevel logLevel, string message)
        {
            Log(logLevel, message, null);
        }

        public void Log(LogLevel logLevel, string message, Exception exception)
        {
            var isException = exception != null;
            switch (logLevel)
            {
                case LogLevel.Trace:
                    if (isException)
                        _internalTarget.LogTrace(exception, message);
                    else
                        _internalTarget.LogTrace(message);
                    break;
                case LogLevel.Debug:
                    if (isException)
                        _internalTarget.LogDebug(exception, message);
                    else
                        _internalTarget.LogDebug(message);
                    break;
                case LogLevel.Info:
                    if (isException)
                        _internalTarget.LogInformation(exception, message);
                    else
                        _internalTarget.LogInformation(message);
                    break;
                case LogLevel.Warning:
                    if (isException)
                        _internalTarget.LogWarning(exception, message);
                    else
                        _internalTarget.LogWarning(message);
                    break;
                case LogLevel.Error:
                    if (isException)
                        _internalTarget.LogError(exception, message);
                    else
                        _internalTarget.LogError(message);
                    break;
                case LogLevel.Fatal:
                    if (isException)
                        _internalTarget.LogCritical(exception, message);
                    else
                        _internalTarget.LogCritical(message);
                    break;
            }
        }
    }
}
