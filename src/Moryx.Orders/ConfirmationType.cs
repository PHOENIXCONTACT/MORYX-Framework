// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders;

/// <summary>
/// Confirmation type while reporting amounts of an operation
/// </summary>
public enum ConfirmationType
{
    /// <summary>
    /// Partial report. The operation will not be completed
    /// </summary>
    Partial = 10,

    /// <summary>
    /// Final report. The operation will be completed and cannot be produced anymore
    /// </summary>
    Final = 20
}