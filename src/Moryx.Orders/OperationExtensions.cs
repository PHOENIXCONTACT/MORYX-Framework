// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders;

/// <summary>
/// Additional extensions of <see cref="Operation"/>
/// </summary>
public static class OperationExtensions
{
    /// <summary>
    /// Sum of reported success of an operation
    /// </summary>
    public static int ReportedSuccessCount(this Operation operation)
    {
        return operation.Reports.Sum(r => r.SuccessCount);
    }

    /// <summary>
    /// Sum of reported failure of an operation
    /// </summary>
    public static int ReportedFailureCount(this Operation operation)
    {
        return operation.Reports.Sum(r => r.FailureCount);
    }
}