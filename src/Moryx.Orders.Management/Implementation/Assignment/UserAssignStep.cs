// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Users;

namespace Moryx.Orders.Management.Assignment
{
    [Component(LifeCycle.Singleton, typeof(IOperationAssignStep), Name = nameof(UserAssignStep))]
    internal class UserAssignStep : IOperationAssignStep
    {
        public IUserManagement UserManagement { get; set; }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public Task<bool> AssignStep(IOperationData operationData, IOperationLogger operationLogger)
        {
            return Task.FromResult(true);
        }

        public Task<bool> RestoreStep(IOperationData operationData, IOperationLogger operationLogger)
        {
            var reports = operationData.Operation.Reports.Where(r => r.User is UserReference);

            foreach (var report in reports)
            {
                var originalUser = UserManagement.GetUser(report.User.Identifier);
                report.User = originalUser;
            }

            return Task.FromResult(true);
        }
    }
}
