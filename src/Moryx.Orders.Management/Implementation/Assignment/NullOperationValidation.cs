// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Orders.Assignment;

namespace Moryx.Orders.Management.Assignment
{
    [Component(LifeCycle.Singleton, typeof(IOperationValidation), Name = nameof(NullOperationValidation))]
    internal class NullOperationValidation : IOperationValidation
    {
        public Task InitializeAsync(OperationValidationConfig config)
        {
            return Task.CompletedTask;
        }

        public Task StartAsync()
        {
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            return Task.CompletedTask;
        }

        public Task<bool> ValidateCreationContext(OrderCreationContext orderContext)
        {
            return Task.FromResult(true);
        }

        public Task<bool> Validate(Operation operation, IOperationLogger operationLogger)
        {
            return Task.FromResult(true);
        }
    }
}
