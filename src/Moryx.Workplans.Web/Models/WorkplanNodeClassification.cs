// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans.Web.Models;

/// <summary>
/// Classification of the steps 
/// </summary>
public enum WorkplanNodeClassification
{
    /// <summary>
    /// Node represents an input connector
    /// </summary>
    Input,

    /// <summary>
    /// Node represents an output connector
    /// </summary>
    Output,

    /// <summary>
    /// Task is executed and performs some sort of execution logic
    /// </summary>
    Execution,

    /// <summary>
    /// Step is internal and only alters the control flow (Split, Join, Conditional, Loop)
    /// </summary>
    ControlFlow,

    /// <summary>
    /// Step holds a subworkplan that is either executed or compiled
    /// </summary>
    Subworkplan,
}