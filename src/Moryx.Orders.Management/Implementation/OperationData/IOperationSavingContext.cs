// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders.Management;

internal interface IOperationSavingContext
{
    Task SaveOperation(IOperationData operationData);

    Task RemoveOperation(IOperationData operationData);
}
