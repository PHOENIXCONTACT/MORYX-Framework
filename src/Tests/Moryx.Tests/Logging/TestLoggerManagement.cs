// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Threading;
using Moryx.Logging;

namespace Moryx.Tests.Logging
{
    public class LogTargetMock : ILogTarget
    {
        public void Log(LogLevel logLevel, string message)
        {
            
        }

        public void Log(LogLevel logLevel, string message, Exception exception)
        {
            
        }
    }

    public class TestLoggerManagement : LoggerManagement
    {
        private readonly List<ILogMessage> _messages = new List<ILogMessage>();

        private readonly ManualResetEventSlim _messageReceivedEvent = new ManualResetEventSlim(false);

        public ManualResetEventSlim MessageReceivedEvent => _messageReceivedEvent;

        public IEnumerable<ILogMessage> Messages
        {
            get
            {
                lock (_messages)
                {
                    return _messages.ToArray();
                }
            }
        }

        protected override ILogTarget CreateLogTarget(string name)
        {
            return new LogTargetMock();
        }

        protected override ModuleLoggerConfig GetLoggerConfig(string name)
        {
            return new ModuleLoggerConfig
            {
                ActiveLevel = LogLevel.Info,
                LoggerName = name
            };
        }

        protected override void ForwardToListeners(ILogMessage logMessage)
        {
            lock (_messages)
            {
                _messages.Add(logMessage);
                _messageReceivedEvent.Set();
            }
        }

        public void ClearMessages()
        {
            lock (_messages)
            {
                _messages.Clear();
            }
        }
    }
}
