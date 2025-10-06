// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Orders.Assignment;

namespace Moryx.Orders.Management.Assignment
{
    [Component(LifeCycle.Singleton, typeof(IOperationAssignStep), Name = nameof(ValidateAssignStep))]
    internal class ValidateAssignStep : IOperationAssignStep
    {
        public IOperationValidation OperationValidation { get; set; }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public Task<bool> AssignStep(IOperationData operationData, IOperationLogger operationLogger)
        {
            return OperationValidation.Validate(operationData.Operation, operationLogger);
        }

        public Task<bool> RestoreStep(IOperationData operationData, IOperationLogger operationLogger)
        {
            return Task.FromResult(true);
        }
    }
}
