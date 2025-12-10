// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading.Tasks;

namespace Moryx.Orders.Management.Tests;

internal class NullOperationSavingContext : IOperationSavingContext
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
