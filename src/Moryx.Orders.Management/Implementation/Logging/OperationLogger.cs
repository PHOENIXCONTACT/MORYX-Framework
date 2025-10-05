// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moryx.Logging;

namespace Moryx.Orders.Management
{
    internal class OperationLogger : IOperationLogger
    {
        private readonly IModuleLogger _logger;
        private readonly IOperationData _operationData;
        private readonly List<OperationLogMessage> _messages = new();

        public IReadOnlyCollection<OperationLogMessage> Messages => _messages;

        public OperationLogger(IModuleLogger logger, IOperationData operationData)
        {
            _logger = logger;
            _operationData = operationData;
        }

        public void Log(LogLevel logLevel, string message, params object[] parameters)
        {
            _logger.Log(logLevel, $"{_operationData.OrderData.Number}-{_operationData.Number}: {message}", parameters);
            _messages.Add(new OperationLogMessage(logLevel, string.Format(message, parameters)));
        }

        public void LogException(LogLevel logLevel, Exception exception, string message, params object[] parameters)
        {
            _logger.Log(logLevel, exception, $"{_operationData.OrderData.Number}-{_operationData.Number}: {message}", parameters);
            _messages.Add(new OperationLogMessage(logLevel, exception, string.Format(message, parameters)));
        }
    }
}
