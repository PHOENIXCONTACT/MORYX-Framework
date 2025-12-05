// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Logging;
using Moryx.Notifications;
using Moryx.Orders.Management.Assignment;

namespace Moryx.Orders.Management.Tests
{
    internal class OperationFactoryMock : IOperationFactory
    {
        private readonly IModuleLogger _logger;
        private readonly IJobHandler _jobHandler;
        private readonly IOperationAssignment _operationAssignment;
        private readonly INotificationAdapter _notificationAdapter;

        public OperationFactoryMock(IModuleLogger logger, IJobHandler jobHandler, IOperationAssignment operationAssignment, INotificationAdapter notificationAdapter)
        {
            _logger = logger;
            _jobHandler = jobHandler;
            _operationAssignment = operationAssignment;
            _notificationAdapter = notificationAdapter;
        }

        public IOperationData Create(IOperationSavingContext savingContext)
        {
            return new OperationData(savingContext)
            {
                Logger = _logger,
                JobHandler = _jobHandler,
                OperationAssignment = _operationAssignment,
                CountStrategy = new DoNotReplaceScrapStrategy(),
                NotificationAdapter = _notificationAdapter
            };
        }

        public void Destroy(IOperationData operationData)
        {
        }
    }
}
