// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders.Management;

/// <summary>
/// API for the <see cref="OperationData"/> to save itself
/// </summary>
internal interface IOperationSavingContext
{
    /// <summary>
    /// Saves the operation
    /// </summary>
    Task SaveOperation(IOperationData operationData);

    /// <summary>
    /// Removes the operation from the database
    /// </summary>
    Task RemoveOperation(IOperationData operationData);
}
