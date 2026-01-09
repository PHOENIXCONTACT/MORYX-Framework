// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.Workplans;

namespace Moryx.AbstractionLayer.Processes;

/// <summary>
/// Context for workplans that execute a process
/// </summary>
public class ProcessWorkplanContext : IWorkplanContext
{
    /// <summary>
    /// Create process context for process
    /// </summary>
    public ProcessWorkplanContext(Process process)
    {
        Process = process;
    }

    /// <summary>
    /// Process the workplan is executed on
    /// </summary>
    public Process Process { get; }

    /// <inheritdoc />
    public virtual bool IsDisabled(IWorkplanStep step)
    {
        return ((IWorkplanRecipe)Process.Recipe).DisabledSteps.Contains(step.Id);
    }
}