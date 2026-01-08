// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders;

/// <summary>
/// Source representation for an operation.
/// Can be used to save information of the origin if a operation will be added
/// The object should be serializable
/// </summary>
public interface IOperationSource
{
    /// <summary>
    /// Type of the operation source
    /// </summary>
    string Type { get; }
}