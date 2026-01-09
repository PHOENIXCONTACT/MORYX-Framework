// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders;

/// <summary>
/// Context for a new advice with all necessary information
/// </summary>
public class AdviceContext
{
    /// <summary>
    /// Amount which are already adviced
    /// </summary>
    public int AdvicedAmount { get; set; }
}