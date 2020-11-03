// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Common.Logging;
using Common.Logging.Simple;
using Moryx.Logging;
using LogLevel = Moryx.Logging.LogLevel;

namespace Moryx.Runtime.Kernel
{
    internal class CommonLoggingLogTarget : ILogTarget
    {
        private readonly ILog _internalTarget;

        public CommonLoggingLogTarget(string name)
        {
            try
            {
                _internalTarget = LogManager.GetLogger(name);
            }
            catch
            {
                _internalTarget = new NoOpLogger();
            }
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
                        _internalTarget.Trace(message, exception);
                    else
                        _internalTarget.Trace(message);
                    break;
                case LogLevel.Debug:
                    if (isException)
                        _internalTarget.Debug(message, exception);
                    else
                        _internalTarget.Debug(message);
                    break;
                case LogLevel.Info:
                    if (isException)
                        _internalTarget.Info(message, exception);
                    else
                        _internalTarget.Info(message);
                    break;
                case LogLevel.Warning:
                    if (isException)
                        _internalTarget.Warn(message, exception);
                    else
                        _internalTarget.Warn(message);
                    break;
                case LogLevel.Error:
                    if (isException)
                        _internalTarget.Error(message, exception);
                    else
                        _internalTarget.Error(message);
                    break;
                case LogLevel.Fatal:
                    if (isException)
                        _internalTarget.Fatal(message, exception);
                    else
                        _internalTarget.Fatal(message);
                    break;
            }
        }
    }
}
