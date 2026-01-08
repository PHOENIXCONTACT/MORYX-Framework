// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Orders.Management.Assignment;

/// <summary>
/// Interface to create an operation.
/// </summary>
internal interface IOperationAssignment : IPlugin
{
    /// <summary>
    /// Creates an operation
    /// </summary>
    void Assign(IOperationData operationData);

    /// <summary>
    /// Execute assignment on existing operation
    /// </summary>
    void Reassign(IOperationData operationData);

    /// <summary>
    /// Restores an operation
    /// </summary>
    Task Restore(IOperationData operationData);
}