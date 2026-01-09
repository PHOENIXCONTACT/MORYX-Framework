// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Orders.Assignment;

namespace Moryx.Orders.Management.Assignment;

[Component(LifeCycle.Singleton, typeof(IOperationValidation), Name = nameof(NullOperationValidation))]
internal class NullOperationValidation : IOperationValidation
{
    public Task InitializeAsync(OperationValidationConfig config, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<bool> ValidateCreationContextAsync(OrderCreationContext orderContext, CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }

    public Task<bool> ValidateAsync(Operation operation, IOperationLogger operationLogger, CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }
}