// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders.Management.Tests;

internal sealed class NullOperationSavingContext : IOperationSavingContext
{
    public Task SaveOperation(IOperationData operationData)
    {
        return Task.CompletedTask;
    }

    public Task RemoveOperation(IOperationData operationData)
    {
        return Task.CompletedTask;
    }
}
